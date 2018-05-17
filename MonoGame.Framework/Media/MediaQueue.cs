// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media
{
	public sealed class MediaQueue
	{
        List<Song> songs = new List<Song>();
        private Random random = new Random();

		public MediaQueue()
		{
			
		}
		
		public Song ActiveSong
		{
			get
			{
				if (songs.Count == 0 || ActiveSongIndex < 0)
					return null;
				
				return songs[ActiveSongIndex];
			}
		}

        public int ActiveSongIndex { get; set; } = -1;

        internal int Count
        {
            get
            {
                return songs.Count;
            }
        }

        public Song this[int index]
        {
            get
            {
                return songs[index];
            }
        }

        internal IEnumerable<Song> Songs
        {
            get
            {
                return songs;
            }
        }

		internal Song GetNextSong(int direction, bool shuffle)
		{
			if (shuffle)
				ActiveSongIndex = random.Next(songs.Count);
			else			
				ActiveSongIndex = MathHelper.Clamp(ActiveSongIndex + direction, 0, songs.Count - 1);
			
			return songs[ActiveSongIndex];
		}
		
		internal void Clear()
		{
			Song song;
			for(; songs.Count > 0; )
			{
				song = songs[0];
#if !DIRECTX
				song.Stop();
#endif
				songs.Remove(song);
			}	
		}

#if !DIRECTX
        internal void SetVolume(float volume)
        {
            int count = songs.Count;
            for (int i = 0; i < count; ++i)
                songs[i].Volume = volume;
        }
#endif

        internal void Add(Song song)
        {
            songs.Add(song);
        }

#if !DIRECTX
        internal void Stop()
        {
            int count = songs.Count;
            for (int i = 0; i < count; ++i)
                songs[i].Stop();
        }
#endif
	}
}

