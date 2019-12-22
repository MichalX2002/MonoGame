using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Framework.Media;
using MonoGame.OpenAL;

#if ANDROID
using System.Globalization;
using Android.Content.PM;
using Android.Content;
using Android.Media;
#endif

#if IOS
using AudioToolbox;
using AVFoundation;
#endif

namespace MonoGame.Framework.Audio
{
    internal sealed class ALController : IDisposable
    {
        private static object _initMutex = new object();
        private static ALController _instance;

        private static bool _closed;
        private IntPtr _context;
        private uint[] _allSources;

#if DESKTOPGL || DIRECTX || ANGLE
        // MacOS & Linux share a limit of 256.
        internal const int MAX_NUMBER_OF_SOURCES = 256;
#elif IOS
        // Reference: http://stackoverflow.com/questions/3894044/maximum-number-of-openal-sound-buffers-on-iphone
        internal const int MAX_NUMBER_OF_SOURCES = 32;
#elif ANDROID
        // Set to the same as OpenAL on iOS
        internal const int MAX_NUMBER_OF_SOURCES = 32;
#endif

#if ANDROID
        private const int DEFAULT_FREQUENCY = 48000;
        private const int DEFAULT_UPDATE_SIZE = 512;
        private const int DEFAULT_UPDATE_BUFFER_COUNT = 2;
#elif DESKTOPGL || DIRECTX
        private static OggStreamer _oggstreamer;
#endif

        private Queue<uint> _availableSources;
        private HashSet<uint> _sourcesInUse;
        private bool _isDisposed;

        public bool SupportsIma4 { get; private set; }
        public bool SupportsAdpcm { get; private set; }
        public bool SupportsEfx { get; private set; }
        public bool SupportsFloat32 { get; private set; }
        public uint Filter { get; private set; }

        public IntPtr Device { get; private set; }
        public EffectsExtension Efx { get; }

        public static ALController Instance
        {
            get
            {
                if (_closed)
                    throw new ObjectDisposedException(nameof(ALController));

                if (_instance == null)
                    throw new AudioHardwareException("OpenAL context has failed to initialize.");

                return _instance;
            }
        }

        /// <summary>
        /// Sets up the hardware resources used by the controller.
        /// </summary>
        private ALController()
        {
            if (AL.NativeLibrary == IntPtr.Zero)
                throw new DllNotFoundException(
                    "Couldn't initialize OpenAL because the native binaries couldn't be found.");

            if (!Open())
                throw new AudioHardwareException(
                    "OpenAL device could not be initialized, see console output for details.");

            Efx = new EffectsExtension(Device);

            if (ALC.IsExtensionPresent(Device, "ALC_EXT_CAPTURE"))
                Microphone.PopulateCaptureDevices();

            // We have hardware here and it is ready

            _allSources = new uint[MAX_NUMBER_OF_SOURCES];
            AL.GenSources(_allSources);
            ALHelper.CheckError("Failed to generate sources.");

            if (Efx.IsAvailable)
                Filter = Efx.GenFilter();

            _availableSources = new Queue<uint>(_allSources);
            _sourcesInUse = new HashSet<uint>();
        }

        public static void EnsureInitialized()
        {
            if (_instance != null)
                return;

            try
            {
                _instance = new ALController();
            }
            catch (DllNotFoundException)
            {
                throw;
            }
            catch (AudioHardwareException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AudioHardwareException("Failed to initialize OpenAL.", ex);
            }

#if DESKTOPGL || DIRECTX
            _oggstreamer = new OggStreamer();
#endif
        }

        /// <summary>
        /// Open the sound device, sets up an audio context, and makes the new context
        /// the current context. Note that this method will stop the playback of
        /// music that was running prior to the game start. If any error occurs, then
        /// the state of the controller is reset.
        /// </summary>
        /// <returns>True if the sound controller was setup, and false if not.</returns>
        private bool Open()
        {
            lock (_initMutex)
            {
                try
                {
                    Device = ALC.OpenDevice(string.Empty);
                }
                catch (DllNotFoundException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new AudioHardwareException("OpenAL device could not be initialized.", ex);
                }

                ALCHelper.CheckError("Could not open OpenAL device.");

                if (Device == IntPtr.Zero)
                    return false;

#if ANDROID
                // Attach activity event handlers so we can pause and resume all playing sounds
                MonoGameAndroidGameView.OnPauseGameThread += Activity_Paused;
                MonoGameAndroidGameView.OnResumeGameThread += Activity_Resumed;

                // Query the device for the ideal frequency and update buffer size so
                // we can get the low latency sound path.

                /*
                The recommended sequence is:

                Check for feature "android.hardware.audio.low_latency" using code such as this:
                import android.content.pm.PackageManager;
                ...
                PackageManager pm = getContext().getPackageManager();
                boolean claimsFeature = pm.hasSystemFeature(PackageManager.FEATURE_AUDIO_LOW_LATENCY);
                Check for API level 17 or higher, to confirm use of android.media.AudioManager.getProperty().
                Get the native or optimal output sample rate and buffer size for this device's primary output stream, using code such as this:
                import android.media.AudioManager;
                ...
                AudioManager am = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
                String sampleRate = am.getProperty(AudioManager.PROPERTY_OUTPUT_SAMPLE_RATE));
                String framesPerBuffer = am.getProperty(AudioManager.PROPERTY_OUTPUT_FRAMES_PER_BUFFER));
                Note that sampleRate and framesPerBuffer are Strings. First check for null and then convert to int using Integer.parseInt().
                Now use OpenSL ES to create an AudioPlayer with PCM buffer queue data locator.

                See http://stackoverflow.com/questions/14842803/low-latency-audio-playback-on-android
                */

                int frequency = DEFAULT_FREQUENCY;
                int updateSize = DEFAULT_UPDATE_SIZE;
                int updateBuffers = DEFAULT_UPDATE_BUFFER_COUNT;
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr1)
                {
                    Android.Util.Log.Debug("OAL", Game.Activity.PackageManager.HasSystemFeature(PackageManager.FeatureAudioLowLatency) ? "Supports low latency audio playback." : "Does not support low latency audio playback.");

                    if (Game.Activity.GetSystemService(Context.AudioService) is AudioManager audioManager)
                    {
                        var result = audioManager.GetProperty(AudioManager.PropertyOutputSampleRate);
                        if (!string.IsNullOrEmpty(result))
                            frequency = int.Parse(result, CultureInfo.InvariantCulture);

                        result = audioManager.GetProperty(AudioManager.PropertyOutputFramesPerBuffer);
                        if (!string.IsNullOrEmpty(result))
                            updateSize = int.Parse(result, CultureInfo.InvariantCulture);
                    }

                    // If 4.4 or higher, then we don't need to double buffer on the application side.
                    // See http://stackoverflow.com/a/15006327
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
                        updateBuffers = 1;
                }
                else
                {
                    Android.Util.Log.Debug("OAL", "Android 4.2 or higher required for low latency audio playback.");
                }
                Android.Util.Log.Debug("OAL", "Using sample rate " + frequency + "Hz and " + updateBuffers + " buffers of " + updateSize + " frames.");

                // These are missing and non-standard ALC constants
                const int AlcFrequency = 0x1007;
                const int AlcUpdateSize = 0x1014;
                const int AlcUpdateBuffers = 0x1015;

                Span<int> attribute = stackalloc int[]
                {
                    AlcFrequency, frequency,
                    AlcUpdateSize, updateSize,
                    AlcUpdateBuffers, updateBuffers,
                    0
                };
#elif IOS
                AVAudioSession.SharedInstance().Init();

                // NOTE: Do not override AVAudioSessionCategory set by the game developer:
                //       see https://github.com/MonoGame/MonoGame/issues/6595

                EventHandler<AVAudioSessionInterruptionEventArgs> handler = delegate (object sender, AVAudioSessionInterruptionEventArgs e)
                {
                    switch (e.InterruptionType)
                    {
                        case AVAudioSessionInterruptionType.Began:
                            AVAudioSession.SharedInstance().SetActive(false);
                            ALC.MakeContextCurrent(IntPtr.Zero);
                            ALC.SuspendContext(_context);
                            break;

                        case AVAudioSessionInterruptionType.Ended:
                            AVAudioSession.SharedInstance().SetActive(true);
                            ALC.MakeContextCurrent(_context);
                            ALC.ProcessContext(_context);
                            break;
                    }
                };

                AVAudioSession.Notifications.ObserveInterruption(handler);

                // Activate the instance or else the interruption handler will not be called.
                AVAudioSession.SharedInstance().SetActive(true);

                int[] attribute = Array.Empty<int>();
#else
                int[] attribute = Array.Empty<int>();
#endif

                _context = ALC.CreateContext(Device, attribute);
                ALCHelper.CheckError("Could not create OpenAL context.");

                if (_context != IntPtr.Zero)
                {
                    ALC.MakeContextCurrent(_context);
                    ALCHelper.CheckError("Could not make OpenAL context current.");

                    SupportsIma4 = AL.IsExtensionPresent("AL_EXT_IMA4");
                    SupportsAdpcm = AL.IsExtensionPresent("AL_SOFT_MSADPCM");
                    SupportsEfx = AL.IsExtensionPresent("AL_EXT_EFX");
                    SupportsFloat32 = AL.IsExtensionPresent("AL_EXT_float32");
                    return true;
                }
                return false;
            }
        }

        public static void DestroyInstance()
        {
            if (_instance != null)
            {
                _instance.Dispose();
                _closed = true;
                _instance = null;
            }
        }

        /// <summary>
        /// Reserves a sound source and return its identifier. If there are no available sources
        /// or the controller was not able to setup the hardware then an
        /// <see cref="InstancePlayLimitException"/> is thrown.
        /// </summary>
        /// <returns>The ID of the reserved sound source.</returns>
        public uint ReserveSource()
        {
            lock (_availableSources)
            {
                if (_availableSources.Count == 0)
                    throw new InstancePlayLimitException();

                uint sourceNumber = _availableSources.Dequeue();
                _sourcesInUse.Add(sourceNumber);

                return sourceNumber;
            }
        }

        public void RecycleSource(uint sourceId)
        {
            lock (_availableSources)
            {
                _sourcesInUse.Remove(sourceId);
                _availableSources.Enqueue(sourceId);
            }
        }

        public double GetSourceCurrentPosition(uint sourceId)
        {
            AL.GetSource(sourceId, ALGetSourcei.SampleOffset, out int pos);
            ALHelper.CheckError("Failed to set source offset.");
            return pos;
        }

#if ANDROID
        void Activity_Paused(MonoGameAndroidGameView view)
        {
            // Pause all currently playing sounds by pausing the mixer
            ALC.DevicePause(_device);
        }

        void Activity_Resumed(MonoGameAndroidGameView view)
        {
            // Resume all sounds that were playing when the activity was paused
            ALC.DeviceResume(_device);
        }
#endif

        /// <summary>
        /// Destroys the AL context and closes the device, when they exist.
        /// </summary>
        private void DestroyContexts()
        {
            ALC.MakeContextCurrent(IntPtr.Zero);

            if (_context != IntPtr.Zero)
            {
                ALC.DestroyContext(_context);
                _context = IntPtr.Zero;
            }

            if (Device != IntPtr.Zero)
            {
                ALC.CloseDevice(Device);
                Device = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Dispose of the OpenALSoundCOntroller.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of the OpenALSoundCOntroller.
        /// </summary>
        /// <param name="disposing">If true, the managed resources are to be disposed.</param>
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
#if DESKTOPGL || DIRECTX
                    if (_oggstreamer != null)
                        _oggstreamer.Dispose();
#endif

                    if (Filter != 0 && Efx.IsAvailable)
                        Efx.DeleteFilter(Filter);

                    SoundEffectInstancePool.DisposeInstances();

                    AL.DeleteSources(_allSources);
                    ALHelper.CheckError("Failed to delete source.");

                    Microphone.StopMicrophones();
                    DestroyContexts();
                }
                _isDisposed = true;
            }
        }

        ~ALController()
        {
            Dispose(false);
        }
    }
}