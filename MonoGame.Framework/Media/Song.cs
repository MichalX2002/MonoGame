// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class Song : IDisposable
    {
        private static float _masterVolume = 1f;
        public static float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (value < 0.0f || value > 1.0f)
                    throw new ArgumentOutOfRangeException();

                if (_masterVolume != value)
                {
                    _masterVolume = value;
                    PlatformMasterVolumeChanged();
                }
            }
        }

        public static ReadOnlyCollection<TimeSpan> UpdateTime
        {
            get
            {
#if DIRECTX || DESKTOPGL
                return OggStreamer.Instance.UpdateTime;
#else
                return null;
#endif
            }
        }

        public event SenderEvent<Song> OnFinish;

        public bool IsDisposed { get; private set; }
        internal string FilePath { get; }
        public string Name { get; }

        public MediaState State => PlatformGetState();
        public TimeSpan Duration => PlatformGetDuration();
        public TimeSpan Position { get => PlatformGetPosition(); set => PlatformSetPosition(value); }

        public float Volume { get => PlatformGetVolume(); set => PlatformSetVolume(value); }
        public float Pitch { get => PlatformGetPitch(); set => PlatformSetPitch(value); }
        public bool IsLooping { get => PlatformGetLooping(); set => PlatformSetLooping(value); }

        internal Song(string fileName, string name)
        {
            FilePath = fileName;
            Name = name ?? Path.GetFileNameWithoutExtension(fileName);
            PlatformInitialize(fileName);
        }

        /// <summary>
        /// Creates a <see cref="Song"/> that can be played by streaming the resource.
        /// </summary>
        /// <param name="uri">The path to the song file.</param>
        /// <param name="name">The name for the song. See <see cref="Name"/>.</param>
        /// <returns></returns>
        public static Song FromUri(Uri uri, string name = null)
        {
            string path = Path.GetFullPath(uri.OriginalString);
            return new Song(path, name);
        }

        public void Play(TimeSpan? startPosition = null)
        {
            PlatformPlay(startPosition);
        }

        public void Pause()
        {
            PlatformPause();
        }

        public void Resume()
        {
            PlatformResume();
        }

        public void Stop()
        {
            PlatformStop();
        }

        void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                PlatformDispose(disposing);
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Song()
        {
            Dispose(false);
        }
    }
}