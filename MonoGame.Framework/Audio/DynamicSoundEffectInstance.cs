// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;

namespace MonoGame.Framework.Audio
{
    /// <summary>
    /// <see cref="SoundEffectInstance"/> where the audio data is provided dynamically at runtime.
    /// </summary>
    public sealed partial class DynamicSoundEffectInstance : SoundEffectInstance
    {
        private const int TargetPendingBufferCount = 3;

        private int _buffersNeeded;
        private int _sampleRate;
        private AudioChannels _channels;
        private SoundState _state;

        #region Properties

        /// <summary>
        /// This value has no effect on <see cref="DynamicSoundEffectInstance"/>.
        /// Trying to set it will throw <see cref="InvalidOperationException"/>.
        /// </summary>
        public override bool IsLooped
        {
            get => false;
            set
            {
                AssertNotDisposed();
                if (value)
                    throw new InvalidOperationException(
                        "IsLooped cannot be set true. Submit looped audio data to implement looping.");
            }
        }

        public override SoundState State
        {
            get
            {
                AssertNotDisposed();
                return _state;
            }
        }

        /// <summary>
        /// Gets the number of audio buffers queued for playback.
        /// </summary>
        public int PendingBufferCount
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetPendingBufferCount();
            }
        }

        /// <summary>
        /// Gets the number of samples queued for playback.
        /// </summary>
        public long BufferedSamples
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetBufferedSamples();
            }
        }

        /// <summary>
        /// The event that occurs when the number of queued audio buffers is low.
        /// </summary>
        /// <remarks>
        /// This event may occur when <see cref="Play()"/> is called or during playback when a buffer is completed.
        /// </remarks>
        public event DatalessEvent<DynamicSoundEffectInstance> BufferNeeded;

        #endregion

        #region Constructors

        /// <param name="sampleRate">Sample rate, in Hertz (Hz).</param>
        /// <param name="channels">Number of channels (mono or stereo).</param>
        public DynamicSoundEffectInstance(int sampleRate, AudioChannels channels)
        {
            SoundEffect.Initialize();
            if (SoundEffect._systemState != SoundEffect.SoundSystemState.Initialized)
                throw new AudioHardwareException("Audio has failed to initialize. Call SoundEffect.Initialize() before sound operation to get more specific errors.");

            if ((sampleRate < 8000) || (sampleRate > 48000))
                throw new ArgumentOutOfRangeException(nameof(sampleRate));
            if ((channels != AudioChannels.Mono) && (channels != AudioChannels.Stereo))
                throw new ArgumentOutOfRangeException(nameof(channels));

            _sampleRate = sampleRate;
            _channels = channels;
            _state = SoundState.Stopped;
            PlatformCreate();

            // This instance is added to the pool so that its volume reflects master volume changes
            // and it contributes to the playing instances limit, but the source/voice is not owned by the pool.
            _isPooled = false;
            _isDynamic = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the duration of an audio buffer of the specified size, based on the settings of this instance.
        /// </summary>
        /// <param name="sampleCount">The amount of samples.</param>
        /// <param name="sampleSize">The size of one sample in bits.</param>
        /// <returns>The playback length of the buffer.</returns>
        public TimeSpan GetSampleDuration(int sampleCount, int sampleSize)
        {
            AssertNotDisposed();
            return SoundEffect.GetSampleDuration(sampleCount, sampleSize, _sampleRate, _channels);
        }

        /// <summary>
        /// Returns the size, in bytes, of a buffer of the specified duration, based on the settings of this instance.
        /// </summary>
        /// <param name="duration">The playback length of the buffer.</param>
        /// <param name="sampleSize">The size of one sample in bits.</param>
        /// <returns>The data size of the buffer, in bytes.</returns>
        public int GetSampleSizeInBytes(TimeSpan duration, int sampleSize)
        {
            AssertNotDisposed();
            return SoundEffect.GetSampleSizeInBytes(duration, sampleSize, _sampleRate, _channels);
        }

        /// <summary>
        /// Plays or resumes the sound instance.
        /// </summary>
        public override void Play()
        {
            AssertNotDisposed();

            if (_state != SoundState.Playing)
            {
                // Ensure that the volume reflects master volume, which is done by the setter.
                Volume = Volume;

                // Add the instance to the pool
                if (!SoundEffectInstancePool.SoundsAvailable)
                    throw new InstancePlayLimitException();

                SoundEffectInstancePool.AddToPlaying(this);

                PlatformPlay();
                _state = SoundState.Playing;

                int tries = TargetPendingBufferCount;
                while (tries > 0)
                {
                    CheckBufferCount();
                    tries--;
                }

                DynamicSoundEffectInstanceManager.AddInstance(this);
            }
        }

        /// <summary>
        /// Pauses playback of the instance.
        /// </summary>
        public override void Pause()
        {
            AssertNotDisposed();
            PlatformPause();
            _state = SoundState.Paused;
        }

        /// <summary>
        /// Resumes playback of the instance.
        /// </summary>
        public override void Resume()
        {
            AssertNotDisposed();

            if (_state != SoundState.Playing)
            {
                Volume = Volume;

                // Add the instance to the pool
                if (!SoundEffectInstancePool.SoundsAvailable)
                    throw new InstancePlayLimitException();
                SoundEffectInstancePool.AddToPlaying(this);
            }

            PlatformResume();
            _state = SoundState.Playing;
        }

        /// <summary>
        /// Stops the instance playback immediately.
        /// </summary>
        /// <remarks>
        /// Calling this also releases all queued buffers.
        /// </remarks>
        public override void Stop()
        {
            Stop(true);
        }

        /// <summary>
        /// Stops the instance playback if the <paramref name="immediate"/> parameter is <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// Calling this releases all queued buffers.
        /// </remarks>
        /// <param name="immediate">When set to <see langword="false"/>, this call has no effect.</param>
        public override void Stop(bool immediate)
        {
            AssertNotDisposed();

            if (immediate)
            {
                DynamicSoundEffectInstanceManager.RemoveInstance(this);

                PlatformStop();
                _state = SoundState.Stopped;

                SoundEffectInstancePool.Return(this);
            }
        }

        /// <summary>
        /// Queues audio data for playback.
        /// </summary>
        /// <remarks>
        /// The span length must conform to alignment requirements for the audio format.
        /// </remarks>
        /// <typeparam name="T">The audio data type which will be converted to bytes.</typeparam>
        /// <param name="data">The span containing audio data.</param>
        /// <param name="depth">The depth of the audio data.</param>
        public unsafe void SubmitBuffer<T>(ReadOnlySpan<T> data, AudioDepth depth)
            where T : unmanaged
        {
            AssertNotDisposed();

            if (data.IsEmpty) throw new ArgumentEmptyException(nameof(data));
            SoundEffect.AssertValidAudioDepth(depth);

            if (sizeof(T) % ((int)depth / 8) != 0)
                throw new ArgumentException(
                    "The data bytes do not align with the audio depth.", nameof(data));

            PlatformSubmitBuffer(data, depth);
        }

        public void SubmitBuffer<T>(Span<T> data, AudioDepth depth)
            where T : unmanaged
        {
            SubmitBuffer((ReadOnlySpan<T>)data, depth);
        }

        #endregion

        #region Non-public Methods

        [DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
        }

        protected override void Dispose(bool disposing)
        {
            PlatformDispose(disposing);
            base.Dispose(disposing);
        }

        private void CheckBufferCount()
        {
            if (PendingBufferCount < TargetPendingBufferCount && _state == SoundState.Playing)
                _buffersNeeded++;
        }

        internal void UpdateQueue()
        {
            // Update the buffers
            PlatformUpdateQueue();

            // Raise the event
            var bufferNeededHandler = BufferNeeded;
            if (bufferNeededHandler != null)
            {
                int eventCount = (_buffersNeeded < 3) ? _buffersNeeded : 3;
                for (int i = 0; i < eventCount; i++)
                    bufferNeededHandler?.Invoke(this);
            }

            _buffersNeeded = 0;
        }

        #endregion
    }
}