// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class Song : IEquatable<Song>, IDisposable
    {
        private int _playCount = 0;
        private TimeSpan _duration = TimeSpan.Zero;

        private static float _masterVolume = 1f;
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
                MediaPlayer.Queue.UpdateMasterVolume();
            }
        }

        public bool IsDisposed { get; private set; }

        internal string FilePath { get; private set; }

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
        public string Name => PlatformGetName();
        public int PlayCount => PlatformGetPlayCount();
        public int Rating => PlatformGetRating();
        public int TrackNumber => PlatformGetTrackNumber();

#if ANDROID || OPENAL || WEB || IOS
        internal delegate void FinishedPlayingHandler(object sender, EventArgs args);
#if !(DESKTOPGL || DIRECTX)
        event FinishedPlayingHandler DonePlaying;
#endif
#endif
        internal Song(string fileName, int durationMS) : this(fileName)
        {
            _duration = TimeSpan.FromMilliseconds(durationMS);
        }

        internal Song(string fileName)
        {
            FilePath = fileName;

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
            return new Song(uri.OriginalString)
            {
                FilePath = name
            };
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Song song)
        {
            return song != null && Name == song.Name;
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
                if (disposing)
                {
                    PlatformDispose(disposing);
                }

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