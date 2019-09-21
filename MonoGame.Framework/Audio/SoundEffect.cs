// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
﻿
using System;
using System.IO;
using MonoGame.Utilities.Memory;

namespace MonoGame.Framework.Audio
{
    /// <summary>Represents a loaded sound resource that can be played.</summary>
    /// <remarks>
    /// <para>
    /// A SoundEffect represents the buffer used to hold audio data and metadata. 
    /// SoundEffectInstances are used to play from SoundEffects. 
    /// Multiple SoundEffectInstance objects can be created and played from the same SoundEffect object.
    /// </para>
    /// <para>
    /// The only limit on the number of loaded SoundEffects is restricted by available memory. 
    /// When a SoundEffect is disposed, all SoundEffectInstances created from it will become invalid.</para>
    /// <para>
    /// SoundEffect.Play() can be used for 'fire and forget' sounds. 
    /// If advanced playback controls like volume or pitch is required, use SoundEffect.CreateInstance().
    /// </para>
    /// </remarks>
    public sealed partial class SoundEffect : IDisposable
    {
        private readonly TimeSpan _duration;

        private static void AssertInitialized()
        {
            if (_systemState != SoundSystemState.Initialized)
                throw new AudioHardwareException(
                    "The audio system has failed to initialize. " +
                    $"Call {nameof(Initialize)} before any audio operation to get more specific errors.");
        }

        #region Internal Constructors

        /// <summary>
        /// Only used by <see cref="FromStream"/>.
        /// </summary>
        /// <param name="stream"></param>
        private SoundEffect(Stream stream)
        {
            Initialize();
            AssertInitialized();
            
            // TODO: add more audio formats
            /*
              The audio has the following restrictions:
              Must be a PCM wave or Ogg Vorbis file
              Can only be mono or stereo
              Must be 8 or 16 bit
              Sample rate must be between 8,000 Hz and 48,000 Hz
            */

            PlatformLoadAudioStream(stream, out _duration);
        }

        // Only used from SoundEffectReader.
        internal SoundEffect(ReadOnlySpan<byte> header, ReadOnlySpan<byte> data,
            int durationMs, int loopStart, int loopLength)
        {
            Initialize();
            AssertInitialized();

            _duration = TimeSpan.FromMilliseconds(durationMs);

            // Peek at the format... handle regular PCM data.
            var format = header.ToInt16();
            if (format == 1)
            {
                var channels = header.Slice(2).ToInt16();
                var sampleRate = header.Slice(4).ToInt32();
                var bitsPerSample = header.Slice(14).ToInt16();
                PlatformInitializePcm(data, bitsPerSample, sampleRate, (AudioChannels)channels, loopStart, loopLength);
                return;
            }

            // Everything else is platform specific.
            PlatformInitializeFormat(header, data, loopStart, loopLength);
        }

        // Only used from XACT WaveBank.
        internal SoundEffect(
            MiniFormatTag codec, ReadOnlySpan<byte> buffer, int channels, int sampleRate,
            int blockAlignment, int loopStart, int loopLength)
        {
            Initialize();
            AssertInitialized();

            // Handle the common case... the rest is platform specific.
            if (codec == MiniFormatTag.Pcm)
            {
                _duration = TimeSpan.FromSeconds((float)buffer.Length / (sampleRate * blockAlignment));
                PlatformInitializePcm(buffer, 16, sampleRate, (AudioChannels)channels, loopStart, loopLength);
                return;
            }

            PlatformInitializeXact(codec, buffer, channels, sampleRate, blockAlignment, loopStart, loopLength, out _duration);
        }

        #endregion

        #region Audio System Initialization

        internal enum SoundSystemState
        {
            NotInitialized,
            Initialized,
            FailedToInitialized
        }

        internal static SoundSystemState _systemState = SoundSystemState.NotInitialized;

        /// <summary>
        /// Initializes the sound system.
        /// This method is automatically called when a <see cref="SoundEffect"/> is loaded,
        /// a <see cref="DynamicSoundEffectInstance"/> is created, or <see cref="Microphone.All "/> is queried.
        /// <para>
        /// You can however call this method manually (preferably inside or before the <see cref="Game"/> constructor) to
        /// catch any exception that may occur during the sound system initialization (and act accordingly).
        /// </para>
        /// </summary>
        public static void Initialize()
        {
            if (_systemState != SoundSystemState.NotInitialized)
                return;

            try
            {
                PlatformInitialize();
                _systemState = SoundSystemState.Initialized;
            }
            catch (Exception)
            {
                _systemState = SoundSystemState.FailedToInitialized;
                throw;
            }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Create a sound effect.
        /// </summary>
        /// <param name="data">The buffer with the sound data.</param>
        /// <param name="sampleRate">The sound data sample rate in hertz.</param>
        /// <param name="channels">The number of channels in the sound data.</param>
        /// <remarks>This only supports uncompressed 16bit PCM wav data.</remarks>
        public SoundEffect(Span<byte> data, int sampleRate, AudioChannels channels)
             : this(data, sampleRate, channels, 0, 0)
        {
        }

        /// <summary>
        /// Create a sound effect.
        /// </summary>
        /// <param name="data">The buffer with the sound data.</param>
        /// <param name="sampleRate">The sound data sample rate in hertz.</param>
        /// <param name="channels">The number of channels in the sound data.</param>
        /// <param name="loopStart">The position where the sound should begin looping in samples.</param>
        /// <param name="loopLength">The duration of the sound data loop in samples.</param>
        /// <remarks>This only supports uncompressed 16bit PCM wav data.</remarks>
        public SoundEffect(
            ReadOnlySpan<byte> data, int sampleRate, AudioChannels channels, int loopStart, int loopLength)
        {
            if (sampleRate < 8000 || sampleRate > 48000)
                throw new ArgumentOutOfRangeException(nameof(sampleRate));
            if ((int)channels != 1 && (int)channels != 2)
                throw new ArgumentOutOfRangeException(nameof(channels));

            if (data.Length == 0)
                throw new ArgumentException("Ensure that the buffer length is non-zero.", nameof(data));

            var blockAlign = (int)channels * 2;
            if ((data.Length % blockAlign) != 0)
                throw new ArgumentException(
                    "Ensure that the buffer meets the block alignment requirements for the number of channels.", nameof(data));

            if (data.Length <= 0)
                throw new ArgumentException("Ensure that the buffer length is greater than zero.", nameof(data));

            if (data.Length % blockAlign != 0)
                throw new ArgumentException(
                    "Ensure that the buffer length meets the block alignment requirements for the number of channels.", nameof(data));

            int totalSamples = data.Length / blockAlign;

            if (loopStart < 0)
                throw new ArgumentException("Value cannot not be negative.", nameof(loopStart));
            if (loopStart > totalSamples)
                throw new ArgumentException("Value cannot be greater than the total number of samples.", nameof(loopStart));

            if (loopLength == 0)
                loopLength = totalSamples - loopStart;

            if (loopLength < 0)
                throw new ArgumentException("Value cannot be negative.", nameof(loopLength));
            if (((ulong)loopStart + (ulong)loopLength) > (ulong)totalSamples)
                throw new ArgumentException(
                    "Ensure that the loopStart+loopLength region lies within the sample range.", nameof(loopLength));

            _duration = GetSampleDuration(data.Length, 16, sampleRate, channels);

            PlatformInitializePcm(data, 16, sampleRate, channels, loopStart, loopLength);
        }

        #endregion

        #region Additional SoundEffect/SoundEffectInstance Creation Methods

        /// <summary>
        /// Creates a sound effect instance for this sound effect.
        /// </summary>
        /// <returns>A new sound effect instance.</returns>
        /// <remarks>
        /// Creating a sound effect instance instead of calling <see cref="SoundEffectInstance.Play"></see>
        /// allows you to access advanced playback features, such as volume, pitch, and 3D positioning.
        /// </remarks>
        public SoundEffectInstance CreateInstance()
        {
            var inst = new SoundEffectInstance();
            PlatformSetupInstance(inst);

            inst._isPooled = false;
            inst._effect = this;

            return inst;
        }

        /// <summary>
        /// Creates a sound effect based on the data from the specified stream.
        /// </summary>
        /// <param name="stream">A stream containing PCM wave or Ogg Vorbis data.</param>
        /// <returns>A new sound effect.</returns>
        /// <remarks>
        /// The stream must point to the start of a valid audio file. The formats supported are:
        /// <list type="bullet">
        /// <item>
        ///     <description>Ogg Vorbis</description>
        ///     <description>8-bit unsigned PCM</description>
        ///     <description>16-bit signed PCM</description>
        ///     <description>24-bit signed PCM</description>
        ///     <description>32-bit IEEE float PCM</description>
        ///     <description>MS-ADPCM 4-bit compressed</description>
        ///     <description>IMA/ADPCM (IMA4) 4-bit compressed</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static SoundEffect FromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            return new SoundEffect(stream);
        }

        /// <summary>
        /// Returns the duration for PCM audio samples.
        /// </summary>
        /// <param name="sampleCount">The amount of samples.</param>
        /// <param name="sampleSize">The size of one sample in bits.</param>
        /// <param name="sampleRate">Sample rate in hertz (Hz). Must be between 8000 Hz and 48000 Hz</param>
        /// <param name="channels">Number of channels in the audio data.</param>
        /// <returns>The duration of the audio data.</returns>
        public static TimeSpan GetSampleDuration(
            int sampleCount, int sampleSize, int sampleRate, AudioChannels channels)
        {
            if (sampleCount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sampleCount), "Element count cannot be negative.");

            if (sampleSize <= 0)
                throw new ArgumentException(
                    "Element size must be greater than zero.", nameof(sampleSize));

            if (sampleRate <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sampleRate), "Sample rate must be greater than zero.");

            int numChannels = (int)channels;
            if (numChannels != 1 && numChannels != 2)
                throw new ArgumentOutOfRangeException(nameof(channels));

            if (sampleCount == 0)
                return TimeSpan.Zero;

            // Reference
            // http://tinyurl.com/hq9slfy

            double dur = sampleCount / (sampleRate * numChannels * sampleSize / 8.0);
            return TimeSpan.FromTicks((long)(dur * TimeSpan.TicksPerSecond));
        }

        /// <summary>
        /// Returns the data size in bytes for PCM audio samples.
        /// </summary>
        /// <param name="duration">The total duration of the audio data.</param>
        /// <param name="sampleSize">The size of one sample in bits.</param>
        /// <param name="sampleRate">Sample rate in hertz (Hz), of audio data. Must be between 8,000 and 48,000 Hz.</param>
        /// <param name="channels">Number of channels in the audio data.</param>
        /// <returns>The size in bytes of a single sample of audio data.</returns>
        public static int GetSampleSizeInBytes(
            TimeSpan duration, int sampleSize, int sampleRate, AudioChannels channels)
        {
            if (duration < TimeSpan.Zero || duration > TimeSpan.FromMilliseconds(0x7FFFFFF))
                throw new ArgumentOutOfRangeException(nameof(duration));
            if (sampleRate < 8000 || sampleRate > 48000)
                throw new ArgumentOutOfRangeException(nameof(sampleRate));

            var numChannels = (int)channels;
            if (numChannels != 1 && numChannels != 2)
                throw new ArgumentOutOfRangeException(nameof(channels));

            // Reference
            // http://tinyurl.com/hq9slfy

            double sizeInBytes = duration.TotalSeconds * (sampleRate * numChannels * sampleSize / 8.0);
            return (int)sizeInBytes;
        }

        #endregion

        #region Play

        /// <summary>Gets a sound effect instance and plays it.</summary>
        /// <returns>true if a instance was successfully played, false if not.</returns>
        /// <remarks>
        /// <para>Play returns false if more instances are currently playing then the platform allows.</para>
        /// <para>To loop a sound or apply 3D effects, use instances from <see cref="CreateInstance"/> instead.</para>
        /// <para>Instances used by SoundEffect.Play() are pooled internally.</para>
        /// </remarks>
        public bool Play()
        {
            var inst = GetPooledInstance(false);
            if (inst == null)
                return false;

            inst.Play();
            return true;
        }

        /// <summary>Gets an sound effect instance and plays it with the specified volume, pitch, and panning.</summary>
        /// <returns>true if a instance was successfully played, false if not.</returns>
        /// <param name="volume">Volume, ranging from 0.0 (silence) to 1.0 (full volume). Volume during playback is scaled by SoundEffect.MasterVolume.</param>
        /// <param name="pitch">Pitch adjustment, ranging from 0.0 (down an octave) to 1.0 (no change) to 2.0 (up an octave).</param>
        /// <param name="pan">Panning, ranging from -1.0 (left speaker) to 0.0 (centered), 1.0 (right speaker).</param>
        /// <remarks>
        /// <para>Play returns false if more instances are currently playing then the platform allows.</para>
        /// <para>To apply looping or simulate 3D audio, use instances from <see cref="CreateInstance"/> instead.</para>
        /// <para>Instances used by SoundEffect.Play() are pooled internally.</para>
        /// </remarks>
        public bool Play(float volume, float pitch, float pan)
        {
            var inst = GetPooledInstance(false);
            if (inst == null)
                return false;

            inst.Volume = volume;
            inst.Pitch = pitch;
            inst.Pan = pan;

            inst.Play();

            return true;
        }

        /// <summary>
        /// Returns a sound effect instance from the pool or null if none are available.
        /// </summary>
        internal SoundEffectInstance GetPooledInstance(bool forXAct)
        {
            if (!SoundEffectInstancePool.SoundsAvailable)
                return null;

            var inst = SoundEffectInstancePool.GetInstance(forXAct);
            inst._effect = this;
            PlatformSetupInstance(inst);

            return inst;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the duration of the SoundEffect.</summary>
        public TimeSpan Duration => _duration;

        /// <summary>Gets or sets the asset name of the SoundEffect.</summary>
        public string Name { get; set; } = string.Empty;

        #endregion

        #region Static Members

        private static float _masterVolume = 1.0f;
        /// <summary>
        /// Gets or sets the master volume scale applied to all SoundEffectInstances.
        /// </summary>
        /// <remarks>
        /// <para>Each SoundEffectInstance has its own Volume property that is independent to SoundEffect.MasterVolume. During playback SoundEffectInstance.Volume is multiplied by SoundEffect.MasterVolume.</para>
        /// <para>This property is used to adjust the volume on all current and newly created SoundEffectInstances. The volume of an individual SoundEffectInstance can be adjusted on its own.</para>
        /// </remarks>
        public static float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (value < 0.0f || value > 1.0f)
                    throw new ArgumentOutOfRangeException();

                if (_masterVolume == value)
                    return;

                _masterVolume = value;
                SoundEffectInstancePool.UpdateMasterVolume();
            }
        }

        private static float _distanceScale = 1.0f;
        /// <summary>
        /// Gets or sets the scale of distance calculations.
        /// </summary>
        /// <remarks> 
        /// <para>DistanceScale defaults to 1.0 and must be greater than 0.0.</para>
        /// <para>Higher values reduce the rate of falloff between the sound and listener.</para>
        /// </remarks>
        public static float DistanceScale
        {
            get => _distanceScale;
            set
            {
                if (value <= 0f)
                    throw new ArgumentOutOfRangeException(nameof(value), "value of DistanceScale: " + value);

                _distanceScale = value;
            }
        }

        private static float _dopplerScale = 1f;
        /// <summary>
        /// Gets or sets the scale of Doppler calculations applied to sounds.
        /// </summary>
        /// <remarks>
        /// <para>DopplerScale defaults to 1.0 and must be greater or equal to 0.0</para>
        /// <para>Affects the relative velocity of emitters and listeners.</para>
        /// <para>Higher values more dramatically shift the pitch for the given relative velocity of the emitter and listener.</para>
        /// </remarks>
        public static float DopplerScale
        {
            get => _dopplerScale;
            set
            {
                // As per documenation it does not look like the value can be less than 0
                //   although the documentation does not say it throws an error we will anyway
                //   just so it is like the DistanceScale
                if (value < 0.0f)
                    throw new ArgumentOutOfRangeException(
                        value.ToString(), "value of DopplerScale");

                _dopplerScale = value;
            }
        }

        private static float speedOfSound = 343.5f;
        /// <summary>Returns the speed of sound used when calculating the Doppler effect..</summary>
        /// <remarks>
        /// <para>Defaults to 343.5. Value is measured in meters per second.</para>
        /// <para>Has no effect on distance attenuation.</para>
        /// </remarks>
        public static float SpeedOfSound
        {
            get => speedOfSound;
            set
            {
                if (value <= 0.0f)
                    throw new ArgumentOutOfRangeException();

                speedOfSound = value;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>Indicates whether the object is disposed.</summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Releases the resources held by this <see cref="SoundEffect"/>.
        /// Instances currently playing will stop immediately and become invalid.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                SoundEffectInstancePool.StopPooledInstances(this);
                PlatformDispose(disposing);
                IsDisposed = true;
            }
        }

        ~SoundEffect()
        {
            Dispose(false);
        }

        #endregion

    }
}
