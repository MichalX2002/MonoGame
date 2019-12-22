using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Framework.Collections
{
    public partial class LongDictionary<TKey, TValue>
    {
        [DebuggerDisplay("Count = {Count}")]
        public sealed class ValueCollection : IReadOnlyCollection<TValue>, IEnumerable<TValue>
        {
            private readonly LongDictionary<TKey, TValue> _dictionary;

            public bool IsReadOnly => true;
            public int Count => _dictionary.Count;

            public ValueCollection(LongDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public bool Contains(TValue item) => _dictionary.ContainsValue(item);

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (array.Length - index < _dictionary.Count)
                    throw new ArgumentException(nameof(array), ArrayTooSmallException);

                int count = _dictionary.count;
                for (int i = 0; i < count; i++)
                {
                    if (_dictionary.entries[i]._hashCode >= 0)
                        array[index++] = _dictionary.entries[i]._value;
                }
            }

            public Enumerator GetEnumerator() => new Enumerator(_dictionary);
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public struct Enumerator : IEnumerator<TValue>
            {
                private readonly LongDictionary<TKey, TValue> _dictionary;
                private readonly int _version;
                private int _index;

                public TValue Current { get; private set; }
                object IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || (_index == _dictionary.count + 1))
                            throw new InvalidOperationException();
                        return Current;
                    }
                }

                internal Enumerator(LongDictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary.version;
                    _index = 0;
                    Current = default;
                }

                public bool MoveNext()
                {
                    if (_version != _dictionary.version)
                        throw new InvalidOperationException(VersionChangedException);

                    while ((uint)_index < (uint)_dictionary.count)
                    {
                        if (_dictionary.entries[_index]._hashCode >= 0)
                        {
                            Current = _dictionary.entries[_index]._value;
                            _index++;
                            return true;
                        }
                        _index++;
                    }
                    _index = _dictionary.count + 1;
                    Current = default;
                    return false;
                }

                void IEnumerator.Reset()
                {
                    if (_version != _dictionary.version)
                        throw new InvalidOperationException(VersionChangedException);

                    _index = 0;
                    Current = default;
                }

                public void Dispose()
                {
                }
            }
        }
    }
}
