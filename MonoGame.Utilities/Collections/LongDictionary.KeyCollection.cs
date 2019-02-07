using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Utilities.Collections
{
    public partial class LongDictionary<TKey, TValue>
    {
        [DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : IReadOnlyCollection<TKey>, IEnumerable<TKey>
        {
            private readonly LongDictionary<TKey, TValue> _dictionary;

            public bool IsReadOnly => true;
            public int Count => _dictionary.Count;

            public KeyCollection(LongDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public bool Contains(in TKey item)
            {
                return _dictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (array.Length - index < _dictionary.Count)
                    throw new ArgumentException(nameof(array), ArrayTooSmallException);

                for (int i = 0; i < _dictionary.count; i++)
                {
                    ref Entry entry = ref _dictionary.entries[i];
                    if (entry._hashCode >= 0)
                        array[index++] = entry._key;
                }
            }

            public Enumerator GetEnumerator() => new Enumerator(_dictionary);
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public struct Enumerator : IEnumerator<TKey>
            {
                private readonly LongDictionary<TKey, TValue> _dictionary;
                private readonly int _version;
                private int _index;

                public TKey Current { get; private set; }
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
                        ref Entry entry = ref _dictionary.entries[_index];
                        if (entry._hashCode >= 0)
                        {
                            Current = entry._key;
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
