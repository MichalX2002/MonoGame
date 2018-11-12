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
            for (int i = songs.Count; i-- > 0; )
            {
#if !DIRECTX
                songs[i].Stop();
#endif
				songs.RemoveAt(i);
			}	
		}

#if !DIRECTX
        internal void SetVolume(float volume)
        {
            for (int i = 0; i < songs.Count; ++i)
                songs[i].Volume = volume;
        }

        internal void SetPitch(float pitch)
        {
            for (int i = 0; i < songs.Count; ++i)
                songs[i].Pitch = pitch;
        }
#endif

        internal void Add(Song song)
        {
            songs.Add(song);
        }

#if !DIRECTX
        internal void Stop()
        {
            for (int i = 0; i < songs.Count; ++i)
                songs[i].Stop();
        }
#endif
	}
}

