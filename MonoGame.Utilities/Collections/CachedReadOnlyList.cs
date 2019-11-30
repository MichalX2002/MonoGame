using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoGame.Utilities.Collections
{
    /// <summary>
    /// Caches a <see cref="ReadOnlyCollection{T}"/> to mitigate allocation from 
    /// repetitive <see cref="List{T}.AsReadOnly"/> calls.
    /// </summary>
    public readonly struct CachedReadOnlyList<T>
    {
        public List<T> List { get; }
        public ReadOnlyCollection<T> ReadOnly { get; }

        public bool IsEmpty => List == null;

        public CachedReadOnlyList(List<T> items)
        {
            List = items ?? throw new ArgumentNullException(nameof(items));
            ReadOnly = List.AsReadOnly();
        }

        /// <summary>
        /// Creates a <see cref="CachedReadOnlyList{T}"/> with a new empty list.
        /// </summary>
        public static CachedReadOnlyList<T> Create()
        {
            return new CachedReadOnlyList<T>(new List<T>());
        }
    }
}
