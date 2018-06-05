using System;
using System.Collections.Concurrent;

namespace MonoGame.Imaging
{
    public class MemoryManager : IDisposable
    {
        private readonly object _mutex;
        private ConcurrentDictionary<long, Pointer> _pointers;
        private readonly bool _clearOnDispose;

        public bool Disposed { get; private set; }
        public long AllocatedBytes => GetAllocatedBytes();
        public int AllocatedPointers => _pointers.Count;

        public MemoryManager(int concurrencyLevel, bool clearOnDispose)
        {
            _mutex = new object();
            _pointers = new ConcurrentDictionary<long, Pointer>(concurrencyLevel, 4);

            _clearOnDispose = clearOnDispose;
        }

        public MemoryManager(bool clearOnDispose) : this(2, clearOnDispose)
        {
        }

        /// <summary>
        /// Disposes all pointers allocated through this <see cref="MemoryManager"/>.
        /// </summary>
        public void Clear()
        {
            CheckDisposed();

            lock (_mutex)
            {
                foreach (var p in _pointers.Values)
                    p.Dispose();

                _pointers.Clear();
            }
        }

        private long GetAllocatedBytes()
        {
            if (Disposed)
                return 0;

            lock (_mutex)
            {
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
            CheckDisposed();

            var result = new MarshalPointer<byte>((int)size);
            _pointers[(long)result.Ptr] = result;

            return result.Ptr;
        }

        internal unsafe void MemMove(void* a, void* b, long size)
        {
            CheckDisposed();

            using (var temp = new MarshalPointer<byte>((int)size))
            {
                Imaging.MemCopy(temp.Ptr, b, size);
                Imaging.MemCopy(a, temp.Ptr, size);
            }
        }

        internal unsafe void Free(void* a)
        {
            CheckDisposed();

            if (_pointers.TryRemove((long)a, out Pointer pointer))
                pointer.Dispose();
        }

        internal unsafe void* ReAlloc(void* a, long newSize)
        {
            CheckDisposed();

            if (!_pointers.TryGetValue((long)a, out Pointer pointer))
            {
                // Allocate new
                return MAlloc(newSize);
            }

            if (newSize <= pointer.Size)
            {
                // Realloc not required
                return a;
            }

            var result = MAlloc(newSize);
            Imaging.MemCopy(result, a, pointer.Size);

            _pointers.TryRemove((long)pointer.Ptr, out pointer);
            pointer.Dispose();

            return result;
        }

        internal unsafe int MemCmp(void* a, void* b, long size)
        {
            CheckDisposed();

            int result = 0;
            byte* ap = (byte*)a;
            byte* bp = (byte*)b;

            for (long i = 0; i < size; ++i)
            {
                if (*ap != *bp)
                    result += 1;

                ap++;
                bp++;
            }

            return result;
        }

        private void CheckDisposed()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MemoryManager));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (_clearOnDispose)
                    Clear();

                _pointers = null;

                Disposed = true;
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
