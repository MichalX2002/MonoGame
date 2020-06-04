// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    /// <summary>
    /// The collection of the <see cref="CurveKey"/> elements and a part of the <see cref="Curve"/> class.
    /// </summary>
    // TODO : [TypeConverter(typeof(ExpandableObjectConverter))]
    [DataContract]
    public class CurveKeyCollection : IList<CurveKey>
    {
        #region Private Fields

        private readonly List<CurveKey> _keys;

        #endregion

        #region Properties

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="index">The index of key in this collection.</param>
        /// <returns><see cref="CurveKey"/> at <paramref name="index"/> position.</returns>
        [DataMember(Name = "Items")]
        public CurveKey this[int index]
        {
            get => _keys[index];
            set => Insert(index, value);
        }

        /// <summary>
        /// Returns the count of keys in this collection.
        /// </summary>
        [DataMember]
        public int Count => _keys.Count;

        /// <summary>
        /// Returns false because it is not a read-only collection.
        /// </summary>
        [DataMember]
        public bool IsReadOnly => false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CurveKeyCollection"/> class.
        /// </summary>
        public CurveKeyCollection()
        {
            _keys = new List<CurveKey>();
        }

        #endregion

        /// <summary>
        /// Adds a key to this collection.
        /// </summary>
        /// <param name="key">New key for the collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <remarks>
        /// The new key would be added respectively to a position of that key and the position of other keys.
        /// </remarks>
        public void Add(CurveKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_keys.Count == 0)
            {
                _keys.Add(key);
                return;
            }

            for (int i = 0; i < _keys.Count; i++)
            {
                if (key.Position < _keys[i].Position)
                {
                    _keys.Insert(i, key);
                    return;
                }
            }

            _keys.Add(key);
        }

        public void Insert(int index, CurveKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            
            if (_keys[index].Position == key.Position)
            {
                _keys[index] = key;
            }
            else
            {
                _keys.RemoveAt(index);
                _keys.Add(key);
            }
        }

        /// <summary>
        /// Removes all keys from this collection.
        /// </summary>
        public void Clear()
        {
            _keys.Clear();
        }

        /// <summary>
        /// Creates a copy of this collection.
        /// </summary>
        /// <returns>A copy of this collection.</returns>
        public CurveKeyCollection Clone()
        {
            var collection = new CurveKeyCollection();
            foreach (CurveKey key in _keys)
                collection.Add(key);
            return collection;
        }

        /// <summary>
        /// Determines whether this collection contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in this collection.</param>
        /// <returns><see langword="true"/> if the key is found; <see langword="false"/> otherwise.</returns>
        public bool Contains(CurveKey key)
        {
            return _keys.Contains(key);
        }

        /// <summary>
        /// Copies the keys of this collection to an array, starting at the array index provided.
        /// </summary>
        /// <param name="array">Destination array where elements will be copied.</param>
        /// <param name="arrayIndex">The zero-based index in the array to start copying from.</param>
        public void CopyTo(CurveKey[] array, int arrayIndex)
        {
            _keys.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Finds element in the collection and returns its index.
        /// </summary>
        /// <param name="key">Element for the search.</param>
        /// <returns>Index of the element; or -1 if item is not found.</returns>
        public int IndexOf(CurveKey key)
        {
            return _keys.IndexOf(key);
        }

        /// <summary>
        /// Removes element at the specified index.
        /// </summary>
        /// <param name="index">The index which element will be removed.</param>
        public void RemoveAt(int index)
        {
            _keys.RemoveAt(index);
        }

        /// <summary>
        /// Removes specific element.
        /// </summary>
        /// <param name="key">The element</param>
        /// <returns>
        /// <see langword="true"/> if item is successfully removed; <see langword="false"/> otherwise. 
        /// This method also returns <see langword="false"/> if item was not found.
        /// </returns>
        public bool Remove(CurveKey key)
        {
            return _keys.Remove(key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the <see cref="CurveKeyCollection"/>.</returns>
        public List<CurveKey>.Enumerator GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        IEnumerator<CurveKey> IEnumerable<CurveKey>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
