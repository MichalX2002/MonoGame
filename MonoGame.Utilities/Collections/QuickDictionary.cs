using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MonoGame.Utilities.Collections
{
    [DebuggerDisplay("Count = {Count}")]
    public unsafe class QuickDictionary<TKey, TValue> : IDisposable, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private struct Entry
        {
            public long hashCode;    // Lower 31 bits of hash code, -1 if unused
            public int next;         // Index of next entry, -1 if last
            public TKey key;         // Key of entry
            public TValue value;     // Value of entry
        }

        private const string VersionChangedException = "Version of the dictionary changed.";
        private const string ArrayTooSmallException = "Passed array is too small.";
        
        private const int MaxPrimeArrayLength = 0x7FEFFFFD;
        private static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        private int* buckets;
        private long bucketsLength;
        private Entry[] entries;
        private int count;
        private int version;
        private int freeList;
        private int freeCount;
        private KeyCollection keys;
        private ValueCollection values;
        private IRefEqualityComparer<TKey> keyComparer;

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

        public TValue this[in TKey key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0)
                    return entries[i].value;
                throw new KeyNotFoundException();
            }
            set
            {
                Insert(key, value, false);
            }
        }

        public QuickDictionary() : this(RefEqualityComparer<TKey>.Default, 0) { }

        public QuickDictionary(IRefEqualityComparer<TKey> comparer) : this(comparer, 0) { }

        public QuickDictionary(IRefEqualityComparer<TKey> comparer, int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            if (capacity > 0)
                Initialize(capacity);

            keyComparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }
        
        public QuickDictionary(
            IRefEqualityComparer<TKey> comparer, IDictionary<TKey, TValue> dictionary) :
            this(comparer, dictionary.Count)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }

        public void Add(in TKey key, in TValue value)
        {
            Insert(key, value, true);
        }

        public void Clear()
        {
            if (count > 0)
            {
                for (long i = 0; i < bucketsLength; i++)
                    buckets[i] = -1;

                Array.Clear(entries, 0, count);
                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }

        public bool ContainsKey(in TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public bool ContainsValue(in TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null)
                        return true;
                }
            }
            else
            {
                var c = EqualityComparer<TValue>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && c.Equals(entries[i].value, value))
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
                ref Entry entry = ref entries[i];
                if (entry.hashCode >= 0)
                {
                    array[index++] = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
                }
            }
        }
        
        private int FindEntry(in TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (buckets != null)
            {
                long hashCode = keyComparer.GetLongHashCode(key) & long.MaxValue;
                long targetBucket = hashCode % bucketsLength;
                int i = buckets[targetBucket];
                for (; i >= 0; i = entries[i].next)
                {
                    ref Entry entry = ref entries[i];
                    if (entry.hashCode == hashCode && keyComparer.EqualsByRef(entry.key, key))
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
                if (prime >= min) return prime;
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

        private void DisposeBuckets()
        {
            if(buckets != null)
                Marshal.FreeHGlobal((IntPtr)buckets);
        }

        private void SetNewBuckets(int size)
        {
            DisposeBuckets();
            buckets = (int*)Marshal.AllocHGlobal(sizeof(int) * size);
            bucketsLength = size;
            for (long i = 0; i < bucketsLength; i++)
                buckets[i] = -1;
        }

        private void Initialize(int capacity)
        {
            int size = GetPrime(capacity);
            SetNewBuckets(size);
            entries = new Entry[size];
            freeList = -1;
        }

        private void Insert(in TKey key, in TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (buckets == null)
                Initialize(0);

            long hashCode = keyComparer.GetLongHashCode(key) & long.MaxValue;
            long targetBucket = hashCode % bucketsLength;
            for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
            {
                ref Entry iEntry = ref entries[i];
                if (iEntry.hashCode == hashCode && keyComparer.EqualsByRef(iEntry.key, key))
                {
                    if (add)
                        throw new ArgumentException("An item with the same key has already been added.");

                    iEntry.value = value;
                    version++;
                    return;
                }

            }

            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                {
                    Resize();
                    targetBucket = hashCode % bucketsLength;
                }
                index = count;
                count++;
            }

            ref Entry entry = ref entries[index];
            entry.hashCode = hashCode;
            entry.next = buckets[targetBucket];
            entry.key = key;
            entry.value = value;

            buckets[targetBucket] = index;
            version++;
        }

        private static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;
            
            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
            {
                return MaxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }

        private void Resize()
        {
            Resize(ExpandPrime(count), false);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            SetNewBuckets(newSize);
            Entry[] newEntries = new Entry[newSize];
            Array.Copy(entries, 0, newEntries, 0, count);
            entries = newEntries;

            if (forceNewHashCodes)
            {
                for (int i = 0; i < count; i++)
                {
                    ref Entry entry1 = ref newEntries[i];
                    if (entry1.hashCode != -1)
                    {
                        entry1.hashCode = (keyComparer.GetLongHashCode(entry1.key) & long.MaxValue);
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                ref Entry entry2 = ref newEntries[i];
                if (entry2.hashCode >= 0)
                {
                    long bucket = entry2.hashCode % newSize;
                    entry2.next = buckets[bucket];
                    buckets[bucket] = i;
                }
            }
        }

        public bool Remove(in TKey key)
        {
            return TryRemove(key, out var temp);
        }

        public bool TryRemove(in TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (buckets != null)
            {
                long hashCode = keyComparer.GetLongHashCode(key) & long.MaxValue;
                long bucket = hashCode % bucketsLength;
                int last = -1;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    ref Entry entry = ref entries[i];
                    if (entry.hashCode == hashCode && keyComparer.EqualsByRef(entry.key, key))
                    {
                        if (last < 0)
                            buckets[bucket] = entry.next;
                        else
                            entries[last].next = entry.next;
                        
                        value = entry.value;

                        entry.hashCode = -1;
                        entry.next = freeList;
                        entry.key = default;
                        entry.value = default;

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

        public bool TryGetValue(in TKey key, out TValue value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = entries[i].value;
                return true;
            }
            value = default;
            return false;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                DisposeBuckets();
                Disposed = true;
            }
        }

        ~QuickDictionary()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private QuickDictionary<TKey, TValue> dictionary;
            private readonly int version;
            private int index;

            internal Enumerator(QuickDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                Current = default;
            }

            public bool MoveNext()
            {
                if (version != dictionary.version)
                    throw new InvalidOperationException(VersionChangedException);

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is Int32.MaxValue
                while ((uint)index < (uint)dictionary.count)
                {
                    ref Entry entry = ref dictionary.entries[index];
                    if (entry.hashCode >= 0)
                    {
                        Current = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
                        index++;
                        return true;
                    }
                    index++;
                }

                index = dictionary.count + 1;
                Current = default;
                return false;
            }

            public KeyValuePair<TKey, TValue> Current { get; private set; }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                        throw new InvalidOperationException();

                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                if (version != dictionary.version)
                    throw new InvalidOperationException(VersionChangedException);

                index = 0;
                Current = default;
            }

            public void Dispose()
            {
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>, IEnumerable<TKey>
        {
            private QuickDictionary<TKey, TValue> dictionary;

            public bool IsReadOnly => true;
            public int Count => dictionary.Count;

            public KeyCollection(QuickDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (array.Length - index < dictionary.Count)
                    throw new ArgumentException(nameof(array), ArrayTooSmallException);
                
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < dictionary.count; i++)
                {
                    ref Entry entry = ref entries[i];
                    if (entry.hashCode >= 0)
                        array[index++] = entry.key;
                }
            }

            public bool Contains(TKey item)
            {
                return dictionary.ContainsKey(item);
            }

            public void Add(TKey item)
            {
                throw new InvalidOperationException();
            }

            public void Clear()
            {
                throw new InvalidOperationException();
            }

            public bool Remove(TKey item)
            {
                throw new InvalidOperationException();
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            
            private struct Enumerator : IEnumerator<TKey>
            {
                private QuickDictionary<TKey, TValue> dictionary;
                private int index;
                private readonly int version;

                public TKey Current { get; private set; }

                object IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                            throw new InvalidOperationException();

                        return Current;
                    }
                }

                internal Enumerator(QuickDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    Current = default;
                }

                public bool MoveNext()
                {
                    if (version != dictionary.version)
                        throw new InvalidOperationException(VersionChangedException);
                    
                    while ((uint)index < (uint)dictionary.count)
                    {
                        ref Entry entry = ref dictionary.entries[index];
                        if (entry.hashCode >= 0)
                        {
                            Current = entry.key;
                            index++;
                            return true;
                        }
                        index++;
                    }

                    index = dictionary.count + 1;
                    Current = default;
                    return false;
                }

                void IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                        throw new InvalidOperationException(VersionChangedException);

                    index = 0;
                    Current = default;
                }

                public void Dispose() { }
            }
        }
        
        [DebuggerDisplay("Count = {Count}")]
        public sealed class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>, IEnumerable<TValue>
        {
            private QuickDictionary<TKey, TValue> dictionary;

            public bool IsReadOnly => true;
            public int Count => dictionary.Count;

            public ValueCollection(QuickDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }
            
            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (array.Length - index < dictionary.Count)
                    throw new ArgumentException(nameof(array), ArrayTooSmallException);

                int count = dictionary.count;
                for (int i = 0; i < count; i++)
                {
                    ref Entry entry = ref dictionary.entries[i];
                    if (entry.hashCode >= 0)
                        array[index++] = entry.value;
                }
            }

            public bool Contains(TValue item)
            {
                return dictionary.ContainsValue(item);
            }

            public void Add(TValue item)
            {
                throw new InvalidOperationException();
            }

            public void Clear()
            {
                throw new InvalidOperationException();
            }

            public bool Remove(TValue item)
            {
                throw new InvalidOperationException();
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            private struct Enumerator : IEnumerator<TValue>
            {
                private QuickDictionary<TKey, TValue> dictionary;
                private int index;
                private readonly int version;

                public TValue Current { get; private set; }

                object IEnumerator.Current

                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                            throw new InvalidOperationException();

                        return Current;
                    }
                }

                internal Enumerator(QuickDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    Current = default;
                }

                public bool MoveNext()
                {
                    if (version != dictionary.version)
                        throw new InvalidOperationException(VersionChangedException);
                    
                    while ((uint)index < (uint)dictionary.count)
                    {
                        ref Entry entry = ref dictionary.entries[index];
                        if (entry.hashCode >= 0)
                        {
                            Current = entry.value;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    Current = default;
                    return false;
                }
                
                void IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                        throw new InvalidOperationException(VersionChangedException);

                    index = 0;
                    Current = default;
                }

                public void Dispose() { }
            }
        }

        
    }
}