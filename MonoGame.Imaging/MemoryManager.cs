using System;
using System.Collections.Generic;

namespace MonoGame.Imaging
{
    public unsafe class MemoryManager
    {
        public const int DEFAULT_ARRAY_SIZE = 1024 * 16;
        
        private int _allocatedArrays;
        private List<WeakReference<byte[]>> _arrayPool;

        public bool Disposed { get; private set; }
        public object SyncRoot { get; }
        
        /// <summary>
        ///  Returns the size of the initial pre-allocated
        ///  array that was created by a constructor.
        /// </summary>
        public int PreAllocatedSize { get; }

        /// <summary>
        ///  Returns the amount of currently allocated memory.
        /// </summary>
        public long AllocatedBytes => GetAllocatedBytes();
        
        /// <summary>
        ///  Returns the amount of arrays currently allocated.
        /// </summary>
        public int AllocatedArrays => _allocatedArrays + (_arrayPool == null ? 0 : _arrayPool.Count);
        
        /// <summary>
        ///  Returns the amount of byte arrays allocated throughout
        ///  this <see cref="MemoryManager"/>'s lifetime. 
        /// </summary>
        public int LifetimeAllocatedArrays { get; private set; }

        /// <summary>
        ///  Returns the amount of bytes allocated throughout
        ///  this <see cref="MemoryManager"/>'s lifetime. 
        /// </summary>
        public long LifetimeAllocatedBytes { get; private set; }

        /// <summary>
        /// Contructs a new <see cref="MemoryManager"/> instance.
        /// </summary>
        /// <param name="arrayPreAllocSize">
        ///  One array of this size gets pre-allocated
        ///  (nothing will be pre-allocated if the value is 0).
        /// </param>
        /// <param name="clearOnDispose">
        ///  Indicates if <see cref="Dispose"/> disposes all pointers
        ///  and removes references to arrays.
        /// </param>
        public MemoryManager(int arrayPreAllocSize)
        {
            SyncRoot = new object();
            PreAllocatedSize = arrayPreAllocSize;
            _arrayPool = new List<WeakReference<byte[]>>();

            if (PreAllocatedSize > 0)
            {
                _arrayPool.Add(new WeakReference<byte[]>(new byte[PreAllocatedSize]));
                LifetimeAllocatedBytes += PreAllocatedSize;
                LifetimeAllocatedArrays++;
            }
        }

        /// <summary>
        /// Contructs a new <see cref="MemoryManager"/> instance.
        /// </summary>
        public MemoryManager() : this(DEFAULT_ARRAY_SIZE)
        {
        }

        /// <summary>
        /// Removes all array references allocated through this <see cref="MemoryManager"/>.
        /// </summary>
        public void Clear()
        {
            lock (SyncRoot)
            {
                _arrayPool.Clear();
            }
        }

        internal byte[] Rent()
        {
            return Rent(DEFAULT_ARRAY_SIZE);
        }
        
        internal byte[] Rent(int size)
        {
            lock (SyncRoot)
            {
                _allocatedArrays++;
                for (int i = _arrayPool.Count; i-- > 0;)
                {
                    var arrayRef = _arrayPool[i];
                    if (arrayRef.TryGetTarget(out byte[] array) == false)
                    {
                        _arrayPool.RemoveAt(i);
                        continue;
                    }

                    if (array.Length >= size)
                    {
                        _arrayPool.RemoveAt(i);
                        return array;
                    }
                }

                unchecked
                {
                    LifetimeAllocatedBytes += size;
                    LifetimeAllocatedArrays++;
                }
                return new byte[size];
            }
        }

        internal void Return(byte[] array)
        {
            if (array == null)
                return;

            lock (SyncRoot)
            {
                if (_allocatedArrays == 0)
                    throw new InvalidOperationException(
                        $"This {nameof(MemoryManager)} cannot free more arrays than it allocated.");

                _allocatedArrays--;
                _arrayPool.Add(new WeakReference<byte[]>(array));
            }
        }

        private long GetAllocatedBytes()
        {
            lock (SyncRoot)
            {
                if (Disposed)
                    return 0;

                long total = 0;
                foreach (var p in _arrayPool)
                {
                    if (p.TryGetTarget(out byte[] array))
                        total += array.Length;
                }
                return total;
            }
        }
    }
}
