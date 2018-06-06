﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    public class MemoryManager : IDisposable
    {
        private readonly bool _clearOnDispose;
        private Dictionary<long, Pointer> _pointers;
        private int _allocatedArrays;
        private List<byte[]> _arrayPool;

        public bool Disposed { get; private set; }
        public object SyncRoot { get; }

        /// <summary>
        ///  Returns the amount of currently allocated unmanaged memory.
        /// </summary>
        public long AllocatedBytes => GetAllocatedBytes();

        /// <summary>
        ///  Returns the amount of pointers currently allocated.
        /// </summary>
        public int AllocatedPointers => _pointers == null ? 0 : _pointers.Count;

        /// <summary>
        ///  Returns the amount of arrays currently allocated.
        /// </summary>
        public int AllocatedArrays => _allocatedArrays + (_arrayPool == null ? 0 : _arrayPool.Count);

        /// <summary>
        ///  Returns the amount of bytes allocated throughout
        ///  this <see cref="MemoryManager"/>'s lifetime. 
        /// </summary>
        public long LifetimeAllocatedBytes { get; private set; }

        /// <summary>
        ///  Returns the amount of pointers allocated throughout
        ///  this <see cref="MemoryManager"/>'s lifetime. 
        /// </summary>
        public int LifetimeAllocatedPointers { get; private set; }

        /// <summary>
        ///  Returns the amount of byte arrays allocated throughout
        ///  this <see cref="MemoryManager"/>'s lifetime. 
        /// </summary>
        public int LifetimeAllocatedArrays { get; private set; }

        public MemoryManager(bool clearOnDispose)
        {
            SyncRoot = new object();
            _pointers = new Dictionary<long, Pointer>();
            _arrayPool = new List<byte[]>();

            _clearOnDispose = clearOnDispose;
        }
        
        /// <summary>
        /// Disposes all memory allocated through this <see cref="MemoryManager"/>.
        /// </summary>
        public void Clear()
        {
            lock (SyncRoot)
            {
                CheckDisposed();

                foreach (var p in _pointers.Values)
                    p.Dispose();

                _pointers.Clear();
                _arrayPool.Clear();
            }
        }

        internal byte[] AllocateByteArray(int size)
        {
            lock (SyncRoot)
            {
                CheckDisposed();
                
                for (int i = 0; i < _arrayPool.Count; i++)
                {
                    if (_arrayPool[i].Length >= size)
                    {
                        byte[] pooledArray = _arrayPool[i];
                        _arrayPool.RemoveAt(i);
                        return pooledArray;
                    }
                }

                _allocatedArrays++;
                LifetimeAllocatedArrays++;

                Console.WriteLine("Created new read buffer of size " + size);

                return new byte[size];
            }
        }

        internal void ReleaseByteArray(byte[] array)
        {
            lock (SyncRoot)
            {
                if (_allocatedArrays == 0)
                    throw new InvalidOperationException(
                        $"This {nameof(MemoryManager)} cannot free more arrays than itself allocated.");

                _allocatedArrays--;

                if (_arrayPool != null)
                {
                    _arrayPool.Add(array);
                    _arrayPool.Sort(CompareArraysByLength);
                }
            }
        }

        private static int CompareArraysByLength(Array x, Array y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null)
                    return 1;
                else
                    return x.Length.CompareTo(y.Length);
            }
        }

        private long GetAllocatedBytes()
        {
            lock (SyncRoot)
            {
                if (Disposed)
                return 0;

                long total = 0;
                foreach (var p in _pointers.Values)
                    total += p.Size;
                return total;
            }
        }

        internal unsafe IntPtr MAllocPtr(long size)
        {
            return (IntPtr)MAlloc(size);
        }

        internal unsafe void* MAlloc(long size)
        {
            lock (SyncRoot)
            {
                CheckDisposed();

                if (size > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(size));

                var result = new MarshalPointer<byte>((int)size);
                _pointers[(long)result.Ptr] = result;

                LifetimeAllocatedBytes += size;
                LifetimeAllocatedPointers++;

                return result.Ptr;
            }
        }

        internal static void Free(MemoryManager manager, IntPtr pointer)
        {
            unsafe
            {
                if (pointer == IntPtr.Zero)
                    return;

                if (manager == null)
                    Marshal.FreeHGlobal(pointer);
                else
                    manager.Free((void*)pointer);
            }
        }

        private unsafe void Remove(void* p)
        {
            long key = (long)p;
            if (_pointers.TryGetValue(key, out Pointer value))
            {
                _pointers.Remove(key);
                value.Dispose();
            }
        }

        internal unsafe void Free(void* p)
        {
            lock (SyncRoot)
            {
                CheckDisposed();
                Remove(p);
            }
        }

        internal unsafe void* ReAlloc(void* p, long newSize)
        {
            lock (SyncRoot)
            {
                CheckDisposed();

                if (!_pointers.TryGetValue((long)p, out Pointer pointer))
                    return MAlloc(newSize); // Allocate new

                if (newSize <= pointer.Size)
                    return p; // Realloc not required

                var newP = MAlloc(newSize);
                Imaging.MemCopy(newP, p, pointer.Size);

                Remove(p);

                return newP;
            }
        }

        private void CheckDisposed()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MemoryManager));
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (SyncRoot)
            {
                if (Disposed == false)
                {
                    if (_clearOnDispose)
                        Clear();
                    
                    _pointers = null;
                    _arrayPool = null;

                    Disposed = true;
                }
            }
        }

        ~MemoryManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}