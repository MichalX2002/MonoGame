// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoGame.Framework.Audio
{
    /// <summary>
    /// Provides microphone capture features. 
    /// </summary>
    public sealed partial class Microphone
    {
        public delegate void BufferReadyDelegate(Microphone source, int sampleCount);

        private static List<Microphone> _allMicrophones = new List<Microphone>();
        private static ReadOnlyCollection<Microphone>? _allMicrophonesRead;

        private TimeSpan _bufferDuration;

        /// <summary>
        /// Returns the default microphone.
        /// </summary>
        public static Microphone? Default { get; internal set; }

        /// <summary>
        /// Returns all available microphones.
        /// </summary>
        public static ReadOnlyCollection<Microphone> All
        {
            get
            {
                if (_allMicrophonesRead == null)
                    _allMicrophonesRead = _allMicrophones.AsReadOnly();
                return _allMicrophonesRead;
            }
        }

        /// <summary>
        /// Event that fires when there is audio data available.
        /// </summary>
        public event BufferReadyDelegate? BufferReady;

        /// <summary>
        /// Returns the friendly name of the microphone.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the sample rate of the captured audio, 44100Hz by default.
        /// </summary>
        public int SampleRate { get; internal set; } = 44100;

        /// <summary>
        /// Returns the state of the microphone. 
        /// </summary>
        public MicrophoneState State { get; internal set; } = MicrophoneState.Stopped;

        /// <summary>
        /// Gets or sets the capture buffer duration. 
        /// <para>
        /// This value must be greater than 100 milliseconds, lower than 1000 milliseconds, 
        /// and must be 10 milliseconds aligned (value % 10 == 0).
        /// </para>
        /// </summary>
        public TimeSpan BufferDuration
        {
            get => _bufferDuration;
            set
            {
                if (value.TotalMilliseconds < 100 || value.TotalMilliseconds > 1000)
                    throw new ArgumentOutOfRangeException(
                        nameof(value), "Buffer duration must be a value between 100 and 1000 milliseconds.");

                if (value.TotalMilliseconds % 10 != 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value), "Buffer duration must be 10ms aligned (value % 10 == 0)");

                _bufferDuration = value;
            }
        }

        /// <summary>
        /// Determines if the microphone is a wired headset.
        /// <para>
        /// Always <see langword="true"/> on mobile and <see langword="false"/> otherwise. See remarks.
        /// </para>
        /// </summary>
        /// <remarks>
        /// XNA could know if a headset microphone was plugged in an Xbox 360 controller but MonoGame can't.
        /// Hence, this is always true on mobile platforms, and always false otherwise.
        /// </remarks>
        public bool IsHeadset =>
#if IOS || ANDROID
            // always true on mobile, this can't be queried on any platform 
            // (it was most probably only set to true if the headset was plugged in an XInput controller)
            true;
#else
            false;
#endif

        internal Microphone(string name)
        {
            Name = name;
            BufferDuration = TimeSpan.FromMilliseconds(1000);
        }

        #region Public Methods

        /// <summary>
        /// Returns the duration based on the size of the buffer (assuming 16-bit PCM data).
        /// </summary>
        /// <param name="sizeInBytes">Size, in bytes</param>
        /// <returns>TimeSpan of the duration.</returns>
        public TimeSpan GetSampleDuration(int sizeInBytes)
        {
            // this should be 10ms aligned
            // this assumes 16bit mono data
            return SoundEffect.GetSampleDuration(sizeInBytes, 16, SampleRate, AudioChannels.Mono);
        }

        /// <summary>
        /// Returns the amount of bytes required to hold the specified duration of 16-bit PCM data. 
        /// </summary>
        /// <param name="duration">TimeSpan of the duration of the sample.</param>
        /// <returns>The amount of bytes required.</returns>
        public int GetSampleSizeInBytes(TimeSpan duration)
        {
            // this should be 10ms aligned
            // this assumes 16bit mono data
            return SoundEffect.GetSampleSizeInBytes(duration, 16, SampleRate, AudioChannels.Mono);
        }

        /// <summary>
        /// Starts microphone capture.
        /// </summary>
        public void Start()
        {
            PlatformStart();
        }

        /// <summary>
        /// Stops microphone capture.
        /// </summary>
        public void Stop()
        {
            PlatformStop();
        }

        /// <summary>
        /// Gets the latest available data from the microphone.
        /// </summary>
        /// <typeparam name="T">The type of the buffer elements.</typeparam>
        /// <param name="buffer">Buffer for the captured 16-bit PCM data.</param>
        /// <returns>The amount of 16-bit samples captured.</returns>
        public unsafe int GetData<T>(Span<T> buffer) 
            where T : unmanaged
        {
            if (sizeof(T) * buffer.Length % 2 != 0)
                throw new ArgumentException(
                    "The buffer size in bytes is not divisible by 2.", nameof(buffer));

            return PlatformGetData(buffer);
        }

        #endregion

        internal static void UpdateMicrophones()
        {
            // querying all running microphones for new samples available
            for (int i = 0; i < _allMicrophones.Count; i++)
                _allMicrophones[i].Update();
        }

        internal static void StopMicrophones()
        {
            // stopping all running microphones before shutting down audio devices
            for (int i = 0; i < _allMicrophones.Count; i++)
                _allMicrophones[i].Stop();
        }
    }
}