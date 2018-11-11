// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media
{
	public class SongCollection : ICollection<Song>, IEnumerable<Song>, IEnumerable, IDisposable
	{
        private IList<Song> _innerList;

        public Song this[int index] => _innerList[index];

        public int Count => _innerList.Count;
        public bool IsReadOnly => false;

        public SongCollection()
        {
            _innerList = new List<Song>();
        }

        public SongCollection(IEnumerable<Song> songs) : this()
        {
            foreach(Song song in songs)
                _innerList.Add(song);
        }
		
		public void Add(Song item)
        {
            if (item == null)
                throw new ArgumentNullException();

            if (_innerList.Count == 0)
            {
                _innerList.Add(item);
                return;
            }

            for (int i = 0; i < _innerList.Count; i++)
            {
                if (item.TrackNumber < _innerList[i].TrackNumber)
                {
                    _innerList.Insert(i, item);
                    return;
                }
            }

            _innerList.Add(item);
        }
		
		public void Clear()
        {
            _innerList.Clear();
        }
        
        public bool Contains(Song item)
        {
            return _innerList.Contains(item);
        }
        
        public void CopyTo(Song[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }
		
		public int IndexOf(Song item)
        {
            return _innerList.IndexOf(item);
        }
        
        public bool Remove(Song item)
        {
            return _innerList.Remove(item);
        }

        public IEnumerator<Song> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        public SongCollection Clone()
        {
            SongCollection sc = new SongCollection();
            foreach (Song song in _innerList)
                sc.Add(song);
            return sc;
        }

        public void Dispose()
        {
        }
    }
}

