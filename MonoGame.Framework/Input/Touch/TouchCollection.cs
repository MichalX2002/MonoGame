// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Framework.Input.Touch
{
    /// <summary>
    /// Provides state information for a touch screen enabled device.
    /// </summary>
    public readonly struct TouchCollection : IReadOnlyList<TouchLocation>
    {
        private readonly TouchLocation[] _collection;

        private TouchLocation[] Collection => _collection ?? Array.Empty<TouchLocation>();

        /// <summary>
        /// Gets whether a touch screen is available.
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchCollection"/> with a pre-determined set of touch locations.
        /// </summary>
        /// <param name="touches">Array of <see cref="TouchLocation"/> items to initialize with.</param>
        /// <param name="isConnected">Whether a touch device is connected.</param>
        public TouchCollection(TouchLocation[] touches, bool isConnected)
        {
            _collection = touches ?? throw new ArgumentNullException(nameof(touches));
            IsConnected = isConnected;
        }

        /// <summary>
        /// Returns a <see cref="TouchLocation"/> specified by the identifier.
        /// </summary>
        public bool FindById(int id, out TouchLocation touchLocation)
        {
            for (int i = 0; i < Collection.Length; i++)
            {
                var location = Collection[i];
                if (location.Id == id)
                {
                    touchLocation = location;
                    return true;
                }
            }
            touchLocation = default;
            return false;
        }

        #region IReadOnlyList<TouchLocation>

        /// <summary>
        /// Returns the index of the first occurrence of specified <see cref="TouchLocation"/> item in the collection.
        /// </summary>
        /// <param name="item"><see cref="TouchLocation"/> to query.</param>
        public int IndexOf(TouchLocation item) => Array.IndexOf(Collection, item);

        /// <summary>
        /// Gets or sets the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">Position of the item.</param>
        public TouchLocation this[int index] => Collection[index];

        /// <summary>
        /// Returns true if specified <see cref="TouchLocation"/> item exists in the collection, false otherwise./>
        /// </summary>
        /// <param name="item">The <see cref="TouchLocation"/> item to query for.</param>
        /// <returns>Returns true if queried item is found, false otherwise.</returns>
        public bool Contains(TouchLocation item)
        {
            for (int i = 0; i < Collection.Length; i++)
                if (item == Collection[i])
                    return true;
            return false;
        }

        /// <summary>
        /// Copies the <see cref="TouchLocation"/>collection to specified array starting from the given index.
        /// </summary>
        /// <param name="array">The array to copy <see cref="TouchLocation"/> items.</param>
        /// <param name="arrayIndex">The starting index of the copy operation.</param>
        public void CopyTo(TouchLocation[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of <see cref="TouchLocation"/> items that exist in the collection.
        /// </summary>
        public int Count => Collection.Length;

        /// <summary>
        /// Returns an enumerator for the <see cref="TouchCollection"/>.
        /// </summary>
        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<TouchLocation> IEnumerable<TouchLocation>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        /// <summary>
        /// Provides the ability to iterate through the touch locations in an <see cref="TouchCollection"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<TouchLocation>
        {
            private readonly TouchCollection _collection;
            private int _position;

            internal Enumerator(TouchCollection collection)
            {
                _collection = collection;
                _position = -1;
            }

            /// <inheritdoc />
            public TouchLocation Current => _collection[_position];
            object IEnumerator.Current => Current;

            /// <inheritdoc />
            public bool MoveNext()
            {
                _position++;
                return _position < _collection.Count;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _position = -1;
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }
        }
    }
}
