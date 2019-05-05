using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Utilities.Collections
{
    [DebuggerDisplay("Count = {Count}")]
    public partial class LongDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private struct Entry
        {
            public long _hashCode;    // Lower 31 bits of hash code, -1 if unused
            public int _next;         // Index of next entry, -1 if last
            public TKey _key;         // Key of entry
            public TValue _value;     // Value of entry
        }

        private const string VersionChangedException = "The dictionary has changed.";
        private const string ArrayTooSmallException = "Array is too small.";
        
        private const int MaxPrimeArrayLength = 0x7FEFFFFD;
        private static readonly int[] primes =
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
        };

        private int[] buckets;
        private Entry[] entries;
        private int count;
        private int version;
        private int freeList;
        private int freeCount;

        private KeyCollection keys;
        private ValueCollection values;
        private ILongEqualityComparer<TKey> keyComparer;

        public bool Disposed { get; private set; }
        public int Count => count - freeCount;

        public KeyCollection Keys
        {
            get
            {
                if (keys == null)
                    keys = new KeyCollection(this);
                return keys;
            }
        }

        public ValueCollection Values
        {
            get
            {
                if (values == null)
                    values = new ValueCollection(this);
                return values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0)
                    return entries[i]._value;
                throw new KeyNotFoundException();
            }
            set => Insert(key, value, false);
        }

        public LongDictionary() : this(LongEqualityComparer<TKey>.Default, 0)
        {
        }

        public LongDictionary(ILongEqualityComparer<TKey> comparer) : this(comparer, 0)
        {
        }

        public LongDictionary(ILongEqualityComparer<TKey> comparer, int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            if (capacity > 0)
                Initialize(capacity);

            keyComparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }
        
        public LongDictionary(
            ILongEqualityComparer<TKey> comparer, IDictionary<TKey, TValue> dictionary) :
            this(comparer, dictionary.Count)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public void Clear()
        {
            if (count > 0)
            {
                for (long i = 0; i < buckets.LongLength; i++)
                    buckets[i] = -1;

                for (int i = 0; i < count; i++)
                    entries[i] = default;

                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i]._hashCode >= 0 && entries[i]._value == null)
                        return true;
                }
            }
            else
            {
                var c = EqualityComparer<TValue>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i]._hashCode >= 0 && c.Equals(entries[i]._value, value))
                        return true;
                }
            }
            return false;
        }

        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (array.Length - index < Count)
                throw new ArgumentException(nameof(array), ArrayTooSmallException);
            
            for (int i = 0; i < count; i++)
            {
                Entry entry = entries[i];
                if (entry._hashCode >= 0)
                    array[index++] = new KeyValuePair<TKey, TValue>(entry._key, entry._value);
            }
        }
        
        private int FindEntry(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (buckets != null)
            {
                long hashCode = keyComparer.GetLongHashCode(key) & long.MaxValue;
                long targetBucket = hashCode % buckets.LongLength;
                int i = buckets[targetBucket];
                for (; i >= 0; i = entries[i]._next)
                {
                    Entry entry = entries[i];
                    if (entry._hashCode == hashCode && keyComparer.Equals(entry._key, key))
                        return i;
                }
            }
            return -1;
        }

        private static bool IsPrime(int candidate) 
        {
            if ((candidate & 1) != 0)
            {
                int limit = (int)Math.Sqrt(candidate);
                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                        return false;
                }
                return true;
            }
            return (candidate == 2);
        }

        private static int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < primes.Length; i++)
            {
                int prime = primes[i];
                if (prime >= min)
                    return prime;
            }

            //outside of our predefined table. 
            //compute the hard way. 
            for (int i = (min | 1); i < int.MaxValue; i += 2)
            {
                if (IsPrime(i) && ((i - 1) % 101 != 0))
                    return i;
            }
            return min;
        }

        private void SetNewBuckets(int size)
        {
            buckets = new int[size];
            for (long i = 0; i < buckets.LongLength; i++)
                buckets[i] = -1;
        }

        private void Initialize(int capacity)
        {
            int size = GetPrime(capacity);
            SetNewBuckets(size);
            entries = new Entry[size];
            freeList = -1;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (buckets == null)
                Initialize(0);

            long hashCode = keyComparer.GetLongHashCode(key) & long.MaxValue;
            long targetBucket = hashCode % buckets.LongLength;
            for (int i = buckets[targetBucket]; i >= 0; i = entries[i]._next)
            {
                if (entries[i]._hashCode == hashCode && keyComparer.Equals(entries[i]._key, key))
                {
                    if (add)
                        throw new ArgumentException("An item with the same key has already been added.");

                    entries[i]._value = value;
                    version++;
                    return;
                }
            }

            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index]._next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                {
                    Resize();
                    targetBucket = hashCode % buckets.LongLength;
                }
                index = count;
                count++;
            }

            entries[index] = new Entry
            {
                _hashCode = hashCode,
                _next = buckets[targetBucket],
                _key = key,
                _value = value
            };

            buckets[targetBucket] = index;
            version++;
        }

        private static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;
            
            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
                return MaxPrimeArrayLength;

            return GetPrime(newSize);
        }

        private void Resize()
        {
            Resize(ExpandPrime(count), false);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            SetNewBuckets(newSize);
            var newEntries = new Entry[newSize];

            Array.Copy(entries, 0, newEntries, 0, count);
            entries = newEntries;

            if (forceNewHashCodes)
            {
                for (int i = 0; i < count; i++)
                {
                    if (newEntries[i]._hashCode != -1)
                        newEntries[i]._hashCode = keyComparer.GetLongHashCode(newEntries[i]._key) & long.MaxValue;
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (newEntries[i]._hashCode >= 0)
                {
                    long bucket = newEntries[i]._hashCode % newSize;
                    newEntries[i]._next = buckets[bucket];
                    buckets[bucket] = i;
                }
            }
        }

        public bool Remove(TKey key)
        {
            return TryRemove(key, out _);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (buckets != null)
            {
                long hashCode = keyComparer.GetLongHashCode(key) & long.MaxValue;
                long bucket = hashCode % buckets.LongLength;
                int last = -1;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i]._next)
                {
                    if (entries[i]._hashCode == hashCode && keyComparer.Equals(entries[i]._key, key))
                    {
                        if (last < 0)
                            buckets[bucket] = entries[i]._next;
                        else
                            entries[last]._next = entries[i]._next;
                        
                        value = entries[i]._value;

                        entries[i] = new Entry
                        {
                            _hashCode = -1,
                            _next = freeList,
                            _key = default,
                            _value = default
                        };

                        freeList = i;
                        freeCount++;
                        version++;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = entries[i]._value;
                return true;
            }
            value = default;
            return false;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private LongDictionary<TKey, TValue> _dictionary;
            private readonly int _version;
            private int _index;

            public KeyValuePair<TKey, TValue> Current { get; private set; }
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

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is Int32.MaxValue
                while ((uint)_index < (uint)_dictionary.count)
                {
                    if (_dictionary.entries[_index]._hashCode >= 0)
                    {
                        Current = new KeyValuePair<TKey, TValue>(_dictionary.entries[_index]._key, _dictionary.entries[_index]._value);
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