using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Framework.Collections
{
    /// <summary>
    /// Caches a <see cref="ReadOnlySet{T}"/> and it's source to mitigate allocation from 
    /// repetitive <see cref="SetExtensions.AsReadOnly"/> calls.
    /// </summary>
    public readonly struct CachedReadOnlySet<T> : IEnumerable<T>
    {
        public IReadOnlySet<T> Source { get; }
        public ReadOnlySet<T> ReadOnly { get; }

        public bool IsEmpty => Source == null;

        public CachedReadOnlySet(IReadOnlySet<T> items)
        {
            Source = items ?? throw new ArgumentNullException(nameof(items));
            ReadOnly = Source.AsReadOnly();
        }

        /// <summary>
        /// Creates a <see cref="CachedReadOnlySet{T}"/> with a new empty set.
        /// </summary>
        public static CachedReadOnlySet<T> Create()
        {
            return new CachedReadOnlySet<T>(new HashSet<T>());
        }

        public ReadOnlySet<T>.Enumerator GetEnumerator() => ReadOnly.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Source.GetEnumerator();
    }
}
