// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework
{
    internal class ReusableItemList<T> : ICollection<T>, IReadOnlyList<T>, IEnumerator<T>
    {
        private readonly List<T> _list = new List<T>();
        private int _iteratorIndex;

        #region ICollection<T> Members

        public void Add(T item)
        {
            if (_list.Count > Count)
            {
                _list[Count] = item;                
            }
            else
            {
                _list.Add(item);
            }

            Count++;
        }
		
		public void Sort(IComparer<T> comparison)
		{
			_list.Sort(comparison);
		}
			
		
		public T GetNewItem()
		{
			if (Count < _list.Count)
			{
				return _list[Count++];
			}
			else
			{
				// Damm...Mono fails in this!
				//return (T) Activator.CreateInstance(typeof(T));
				return default;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= Count) 
					throw new IndexOutOfRangeException();
				return _list[index];
			}
			set
			{
				if (index >= Count) 
					throw new IndexOutOfRangeException();
				_list[index] = value;
			}
		}
		
        public void Clear()
        {
            Count = 0;
        }

        public void Reset()
        {
            Clear();
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array,arrayIndex);
        }

        public int Count { get; private set; } = 0;

        public bool IsReadOnly => false;

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            _iteratorIndex = -1;
            return this;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            _iteratorIndex = -1;
            return this;
        }

        #endregion

        #region IEnumerator<T> Members

        public T Current => _list[_iteratorIndex];

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current => _list[_iteratorIndex];

        public bool MoveNext()
        {
            _iteratorIndex++;
            return (_iteratorIndex < Count);
        }

        #endregion
    }
}
