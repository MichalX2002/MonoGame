// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media
{
	public sealed class MediaQueue
	{
        private Random random = new Random();
        
        internal int Count => Songs.Count;
        internal List<Song> Songs { get; } = new List<Song>();

        public Song this[int index] => Songs[index];

        public int ActiveSongIndex { get; set; } = -1;
		
		public Song ActiveSong
		{
			get
			{
				if (Songs.Count == 0 || ActiveSongIndex < 0)
					return null;
				
				return Songs[ActiveSongIndex];
			}
		}

		public MediaQueue()
		{
		}

        internal Song GetNextSong(int direction, bool shuffle)
		{
			if (shuffle)
				ActiveSongIndex = random.Next(Songs.Count);
			else			
				ActiveSongIndex = MathHelper.Clamp(ActiveSongIndex + direction, 0, Songs.Count - 1);
			
			return Songs[ActiveSongIndex];
		}
		
		internal void Clear()
		{
            for (int i = Songs.Count; i-- > 0; )
            {
                Songs[i].Stop();
				Songs.RemoveAt(i);
			}	
		}
        
        internal void SetVolume(float volume)
        {
            for (int i = 0; i < Songs.Count; ++i)
                Songs[i].Volume = volume;
        }

        internal void SetPitch(float pitch)
        {
            for (int i = 0; i < Songs.Count; ++i)
                Songs[i].Pitch = pitch;
        }

        internal void Add(Song song)
        {
            Songs.Add(song);
        }
        
        internal void Stop()
        {
            for (int i = 0; i < Songs.Count; ++i)
                Songs[i].Stop();
        }

        internal void UpdateMasterVolume()
        {
            for (int i = 0; i < Songs.Count; ++i)
            {
                var song = Songs[i];
                song.Volume = song.Volume;
            }
        }
	}
}

