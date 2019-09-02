using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Utilities.Collections
{
    [DebuggerDisplay("Count = {Count}")]
    public class ReadOnlySet<T> : ISet<T>
    {
        private readonly ISet<T> _set;
        private readonly IEqualityComparer<T> _comparer;

        /// <summary>
        /// Gets whether this set always contains the same elements
        /// (i.e was constructed by copying elements from a enumerable).
        /// </summary>
        public bool IsImmutable { get; }

        public bool IsReadOnly => IsImmutable || _set.IsReadOnly;
        public int Count => _set.Count;

        /// <summary>
        /// Constructs a <see cref="ReadOnlySet{T}"/> by wrapping around a <see cref="ISet{T}"/>.
        /// </summary>
        /// <param name="set">The set to wrap around.</param>
        public ReadOnlySet(ISet<T> set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            while (set is ReadOnlySet<T> roSet)
                set = roSet._set;
            _set = set;
        }

        /// <summary>
        /// Constructs a <see cref="ReadOnlySet{T}"/> by copying elements from an <see cref="IEnumerable{T}"/>.
        /// <para>
        /// If the <paramref name="enumerable"/> is an immutable <see cref="ReadOnlySet{T}"/> and 
        /// has the same <see cref="IEqualityComparer{T}"/>, it's backing store will be reused.
        /// </para>
        /// </summary>
        /// <param name="enumerable">The enumerable whose elements are copied from.</param>
        /// <param name="comparer">
        /// The comparer to use when comparing values in the set,
        /// or <see langword="null"/> to use <see cref="EqualityComparer{T}.Default"/>.
        /// </param>
        public ReadOnlySet(IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (enumerable is ReadOnlySet<T> roSet && roSet.IsImmutable && roSet._comparer == _comparer)
                _set = roSet._set;
            else
                _set = new HashSet<T>(enumerable, comparer);

            _comparer = comparer;
            IsImmutable = true;
        }

        public bool Add(T item) => _set.Add(item);
        void ICollection<T>.Add(T item) => _set.Add(item);
        public void Clear() => _set.Clear();
        public bool Contains(T item) => _set.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _set.CopyTo(array, arrayIndex);
        public void ExceptWith(IEnumerable<T> other) => _set.ExceptWith(other);
        public void IntersectWith(IEnumerable<T> other) => _set.IntersectWith(other);
        public bool IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf(other);
        public bool Overlaps(IEnumerable<T> other) => _set.Overlaps(other);
        public bool Remove(T item) => _set.Remove(item);
        public bool SetEquals(IEnumerable<T> other) => _set.SetEquals(other);
        public void SymmetricExceptWith(IEnumerable<T> other) => _set.SymmetricExceptWith(other);
        public void UnionWith(IEnumerable<T> other) => _set.UnionWith(other);

        public Enumerator GetEnumerator() => new Enumerator(_set);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _set.GetEnumerator();

        public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private HashSet<T>.Enumerator _hashSetEnumerator;
            private IEnumerator<T> _genericEnumerator;
            private IEnumerator _hashSetBoxed;

            internal Enumerator(ISet<T> set)
            {
                if (set is HashSet<T> hashSet)
                {
                    _hashSetEnumerator = hashSet.GetEnumerator();
                    _genericEnumerator = null;
                }
                else
                {
                    _hashSetEnumerator = default;
                    _genericEnumerator = set.GetEnumerator();
                }
                _hashSetBoxed = null;
            }

            public T Current
            {
                get
                {
                    if (_genericEnumerator != null)
                        return _genericEnumerator.Current;
                    return _hashSetEnumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_genericEnumerator != null)
                        return _genericEnumerator.Current;
                    return GetBoxed().Current;
                }
            }

            public bool MoveNext()
            {
                if (_genericEnumerator != null)
                    return _genericEnumerator.MoveNext();
                return _hashSetEnumerator.MoveNext();
            }

            void IEnumerator.Reset()
            {
                if (_genericEnumerator != null)
                    _genericEnumerator.Reset();
                else
                    GetBoxed().Reset();
            }

            public void Dispose()
            {
                if (_genericEnumerator != null)
                    _genericEnumerator.Dispose();
                else
                    _hashSetEnumerator.Dispose();
            }

            private IEnumerator GetBoxed()
            {
                if (_hashSetBoxed == null)
                    _hashSetBoxed = _hashSetEnumerator as IEnumerator;
                return _hashSetBoxed;
            }
        }
    }
}
