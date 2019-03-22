// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class Song : IEquatable<Song>, IDisposable
    {
        private int _playCount = 0;
        private TimeSpan? _duration;
        internal string _name;

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
                    MediaPlayer.Queue.UpdateMasterVolume();
                }
            }
        }

        public bool IsDisposed { get; private set; }

        internal string FilePath { get; private set; }
        public string Name { get; }

        /// <summary>
        /// Gets the Album on which the Song appears.
        /// </summary>
        public Album Album => PlatformGetAlbum();

        /// <summary>
        /// Gets the Artist of the Song.
        /// </summary>
        public Artist Artist => PlatformGetArtist();

        /// <summary>
        /// Gets the Genre of the Song.
        /// </summary>
        public Genre Genre => PlatformGetGenre();

        public TimeSpan Duration => PlatformGetDuration();
        public bool IsProtected => PlatformIsProtected();
        public bool IsRated => PlatformIsRated();
        public int PlayCount => PlatformGetPlayCount();
        public int Rating => PlatformGetRating();
        public int TrackNumber => PlatformGetTrackNumber();

#if ANDROID || OPENAL || WEB || IOS
        internal delegate void FinishedPlayingHandler();
#if !(DESKTOPGL || DIRECTX)
        event FinishedPlayingHandler DonePlaying;
#endif
#endif

        internal Song(string fileName, string name, int durationMS) : this(fileName, name)
        {
            _duration = TimeSpan.FromMilliseconds(durationMS);
        }

        internal Song(string fileName, string name)
        {
            FilePath = fileName;
            Name = name ?? Path.GetFileNameWithoutExtension(fileName);
            PlatformInitialize(fileName);
        }

        /// <summary>
        /// Returns a song that can be played via <see cref="MediaPlayer"/>.
        /// </summary>
        /// <param name="name">The name for the song. See <see cref="Name"/>.</param>
        /// <param name="uri">The path to the song file.</param>
        /// <returns></returns>
        public static Song FromUri(string name, Uri uri)
        {
            string path = Path.GetFullPath(uri.OriginalString);
            return new Song(path, name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Song song)
        {
            return 
                song != null && 
                FilePath == song.FilePath &&
                Name == song.Name;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is Song song)
                return Equals(song);
            return false;
        }

        public static bool operator ==(Song song1, Song song2)
        {
            if (song1 is null)
                return song2 is null;

            return song1.Equals(song2);
        }

        public static bool operator !=(Song song1, Song song2)
        {
            return !(song1 == song2);
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