// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace MonoGame.Framework.Media
{
    /// <summary>
    /// Encapsulates a streamable resource that can be used to play audio.
    /// </summary>
    public sealed partial class Song : IDisposable
    {
        #region Static Properties

        private static float _masterVolume = 1f;

        /// <summary>
        /// Gets the master volume which acts for every <see cref="Song"/>.
        /// </summary>
        public static float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (value < 0f || value > 1f)
                    throw new ArgumentOutOfRangeException();
                
                if (_masterVolume != value)
                {
                    _masterVolume = value;
                    PlatformMasterVolumeChanged();
                }
            }
        }

        /// <summary>
        /// Gets 
        /// </summary>
        public static ReadOnlyCollection<TimeSpan> UpdateTime
        {
            get =>
#if DIRECTX || DESKTOPGL
                OggStreamer.Instance.UpdateTime;
#else
                return null;
#endif

        }

        #endregion

        /// <summary>
        /// Occurs when the <see cref="Song"/> stops though not when the it loops.
        /// </summary>
        public event DatalessEvent<Song> Finished;

        /// <summary>
        /// Gets whether the <see cref="Song"/> is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the name of the <see cref="Song"/>.
        /// </summary>
        public string Name { get; }

        #region Method Properties

        public MediaState State
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetState();
            }
        }

        public TimeSpan Duration
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetDuration();
            }
        }

        public TimeSpan Position
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetPosition();
            }
            set
            {
                AssertNotDisposed();
                PlatformSetPosition(value);
            }
        }

        public float Volume
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetVolume();
            }
            set
            {
                AssertNotDisposed();
                PlatformSetVolume(value);
            }
        }

        public float Pitch
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetPitch();
            }
            set
            {
                AssertNotDisposed();
                PlatformSetPitch(value);
            }
        }

        public bool IsLooped
        {
            get
            {
                AssertNotDisposed();
                return PlatformGetLooped();
            }
            set
            {
                AssertNotDisposed();
                PlatformSetLooped(value);
            }
        }

        #endregion

        private Song(Stream stream, bool leaveOpen, string name)
        {
            Name = name;
            PlatformInitialize(stream, leaveOpen);
        }

        /// <summary>
        /// Creates a <see cref="Song"/> that is streamed from a seekable stream.
        /// </summary>
        /// <param name="stream">The seekable stream.</param>
        /// <param name="leaveOpen">true to leave the stream open after disposal; false to also dispose it.</param>
        /// <param name="name">The name for the song.</param>
        public static Song FromStream(Stream stream, bool leaveOpen, string name)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("The stream is not seekable.");
            return new Song(stream, leaveOpen, name);
        }

        /// <summary>
        /// Creates a <see cref="Song"/> that is streamed from a file.
        /// </summary>
        /// <param name="uri">The path to the song file.</param>
        /// <param name="name">The name for the song. See <see cref="Name"/>.</param>
        public static Song FromUri(Uri uri, string name = null)
        {
            string path = Path.GetFullPath(uri.OriginalString);
            name ??= Path.GetFileNameWithoutExtension(path);
            return FromStream(File.OpenRead(path), leaveOpen: false, name);
        }

        public void Play(bool immediate, TimeSpan? startPosition = null)
        {
            AssertNotDisposed();

            if (startPosition.HasValue)
                if (startPosition.Value > Duration)
                    throw new ArgumentOutOfRangeException(
                        nameof(startPosition), "Position exceeds the duration of the song.");

            PlatformPlay(immediate, startPosition);
        }

        public void Play(TimeSpan? startPosition = null)
        {
            Play(immediate: true, startPosition);
        }

        public void Pause()
        {
            AssertNotDisposed();
            PlatformPause();
        }

        public void Resume()
        {
            AssertNotDisposed();
            PlatformResume();
        }

        public void Stop()
        {
            AssertNotDisposed();
            PlatformStop();
        }

        #region IDisposable

        [DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Song));
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                PlatformDispose(disposing);
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Releases resources used by the <see cref="Song"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="Song"/>.
        /// </summary>
        ~Song()
        {
            Dispose(false);
        }

        #endregion
    }
}