// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Framework.Audio
{
    /// <summary>
    ///  Represents a single instance of a playing, paused, or stopped sound.
    ///  </summary>
    /// <remarks>
    /// Instances are created through <see cref="SoundEffect.CreateInstance()"/>
    /// and used internally by <see cref="SoundEffect.Play()"/>.
    /// </remarks>
    public partial class SoundEffectInstance : IDisposable
    {
        internal bool _isPooled = true;
        internal bool _isXAct;
        internal bool _isDynamic;
        internal SoundEffect? _effect;
        private float _pan;
        private float _volume;
        private float _pitch;

        /// <summary>
        /// Gets whether this sound instance is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>Gets the current playback state.</summary>
        public virtual SoundState State => PlatformGetState();

        /// <summary>Gets or sets whether the instance should repeat after playback.</summary>
        /// <remarks>This value has no effect on an already playing sound.</remarks>
        public virtual bool IsLooped
        {
            get => PlatformGetIsLooped();
            set => PlatformSetIsLooped(value);
        }

        /// <summary>Gets or sets the pan, or speaker balance.</summary>
        /// <value>
        /// Ranges from -1 (left speaker) to 0 (centered) to 1 (right speaker).
        /// Values outside of this range will throw an exception.
        /// </value>
        public float Pan
        {
            get => _pan;
            set
            {
                if (value < -1f || value > 1f)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _pan = value;
                PlatformSetPan(value);
            }
        }

        /// <summary>Gets or sets the pitch adjustment.</summary>
        public float Pitch
        {
            get => _pitch;
            set
            {
                // XAct sounds effects don't have pitch limits
                if (!_isXAct && value <= 0f)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _pitch = value;
                PlatformSetPitch(value);
            }
        }

        /// <summary>Gets or sets the volume.</summary>
        /// <value>
        /// Volume, ranging from 0 (silence) to 1 (full volume).
        /// Volume is scaled by <see cref="SoundEffect.MasterVolume"/>.
        /// </value>
        public float Volume
        {
            get => _volume;
            set
            {
                // XAct sound effects don't have volume limits.
                if (!_isXAct && (value < 0f || value > 1f))
                    throw new ArgumentOutOfRangeException(nameof(value));

                _volume = value;
                UpdateVolume();
            }
        }

        internal SoundEffectInstance()
        {
            _pan = 0f;
            _volume = 1f;
            _pitch = 0f;
        }

        /// <summary>Applies 3D positioning to the instance using a single listener.</summary>
        /// <param name="listener">Data about the listener.</param>
        /// <param name="emitter">Data about the source of emission.</param>
        public void Apply3D(AudioListener listener, AudioEmitter emitter)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            if (emitter == null)
                throw new ArgumentNullException(nameof(emitter));

            PlatformApply3D(listener, emitter);
        }

        /// <summary>Applies 3D positioning to the instance using multiple listeners.</summary>
        /// <param name="listeners">Data about each listener.</param>
        /// <param name="emitter">Data about the source of emission.</param>
        public void Apply3D(ReadOnlySpan<AudioListener> listeners, AudioEmitter emitter)
        {
            if (emitter == null)
                throw new ArgumentNullException(nameof(emitter));

            foreach (var listener in listeners)
            {
                if (listener == null)
                    continue;

                PlatformApply3D(listener, emitter);
            }
        }

        /// <summary>Pauses playback of the instance.</summary>
        /// <remarks>Paused instances can be resumed with <see cref="Play()"/> or <see cref="Resume()"/>.</remarks>
        public virtual void Pause()
        {
            PlatformPause();
        }

        /// <summary>Plays or resumes the instance.</summary>
        /// <exception cref="InstancePlayLimitException">More sounds are playing than the platform allows.</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public virtual void Play()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(SoundEffectInstance));

            if (State == SoundState.Playing)
                return;

            if (State == SoundState.Paused)
            {
                Resume();
                return;
            }

            // We don't need to check if we're at the instance play limit
            // if we're resuming from a paused state.
            if (State != SoundState.Paused)
            {
                if (!SoundEffectInstancePool.SlotsAvailable)
                    throw new InstancePlayLimitException();
            }

            // For non-XAct sounds we need to be sure the latest
            // master volume level is applied before playback.
            UpdateMasterVolume();
            PlatformPlay();
            SoundEffectInstancePool.Register(this);
        }

        /// <summary>Resumes playback for the instance.</summary>
        /// <remarks>Only has effect on paused instances.</remarks>
        public virtual void Resume()
        {
            PlatformResume();
        }

        /// <summary>Stops the instance playback immediately.</summary>
        public virtual void Stop()
        {
            PlatformStop(true);
        }

        /// <summary>Stops the instance playback, either immediately or as authored.</summary>
        /// <param name="immediate">
        /// Determines whether the instance stops immediately, 
        /// or after playing its release phase and/or transitions.
        /// </param>
        public virtual void Stop(bool immediate)
        {
            PlatformStop(immediate);
        }

        internal void UpdateMasterVolume()
        {
            PlatformSetVolume(_volume * SoundEffect.MasterVolume);
        }

        internal void UpdateVolume()
        {
            // XAct sound effects are not tied to the SoundEffect master volume.
            if (_isXAct)
                PlatformSetVolume(_volume);
            else
                UpdateMasterVolume();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                PlatformStop(true);
                PlatformDispose(disposing);

                IsDisposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="SoundEffectInstance"/>.
        /// </summary>
        ~SoundEffectInstance()
        {
            Dispose(false);
        }
    }
}
