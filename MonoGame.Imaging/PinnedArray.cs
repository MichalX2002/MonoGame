using System;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    internal abstract unsafe class Pointer : IDisposable
    {
        protected static long _allocatedTotal;
        protected static object _lock = new object();

        public abstract long Size { get; }
        public abstract void* Ptr { get; }

        public static long AllocatedTotal
        {
            get { return _allocatedTotal; }
        }

        public abstract void Dispose();

        public static implicit operator void*(Pointer ptr)
        {
            return ptr.Ptr;
        }

        public static implicit operator byte*(Pointer ptr)
        {
            return (byte*) ptr.Ptr;
        }

        public static implicit operator short*(Pointer ptr)
        {
            return (short*) ptr.Ptr;
        }
    }

    internal unsafe class PinnedArray<T> : Pointer
    {
        private bool _disposed;
        private void* _ptr;
        private long _size;

        public GCHandle Handle { get; }

        public override void* Ptr
        {
            get { return _ptr; }
        }

        public T[] Data { get; private set; }

        public T this[long index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        public long Count { get; private set; }

        public override long Size
        {
            get { return _size; }
        }

        public long ElementSize { get; private set; }

        public PinnedArray(long size)
            : this(new T[size])
        {
        }

        public PinnedArray(T[] data)
        {
            Data = data;

            _ptr = null;
            if (data != null)
            {
                Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                var addr = Handle.AddrOfPinnedObject();
                _ptr = addr.ToPointer();
                ElementSize = Marshal.SizeOf(typeof (T));
                Count = data.Length;
                _size = ElementSize*data.Length;
            }
            else
            {
                ElementSize = 0;
                Count = 0;
                _size = 0;
            }

            lock (_lock)
            {
                _allocatedTotal += _size;
            }
        }

        ~PinnedArray()
        {
            Dispose(false);
        }

        public override void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            lock (_lock)
            {
                _allocatedTotal -= Size;
            }

            if (Data != null)
            {
                Handle.Free();
                _ptr = null;
                Data = null;
                _size = 0;
            }

            _disposed = true;
        }
    }
}
