using System;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    internal abstract unsafe class Pointer : IDisposable
    {
        public abstract int Size { get; }
        public abstract void* Ptr { get; }

        public static implicit operator void* (Pointer ptr)
        {
            return ptr.Ptr;
        }

        public static implicit operator byte* (Pointer ptr)
        {
            return (byte*)ptr.Ptr;
        }

        public static implicit operator short* (Pointer ptr)
        {
            return (short*)ptr.Ptr;
        }

        public static implicit operator ushort* (Pointer ptr)
        {
            return (ushort*)ptr.Ptr;
        }

        public static implicit operator int* (Pointer ptr)
        {
            return (int*)ptr.Ptr;
        }

        public abstract void Dispose();
    }

    internal unsafe class MarshalPointer<TStruct> : Pointer where TStruct : struct
    {
        private bool _disposed;
        private void* _ptr;
        private int _size;

        public override void* Ptr => _ptr;
        public override int Size => _size;

        public int ElementSize { get; private set; }

        public MarshalPointer(int size)
        {
            ElementSize = Marshal.SizeOf(typeof(TStruct));
            _size = size * ElementSize;

            _ptr = (void*)Marshal.AllocHGlobal(_size);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                _size = 0;

                if (_ptr != null)
                {
                    Marshal.FreeHGlobal((IntPtr)_ptr);
                    _ptr = null;
                }

                _disposed = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MarshalPointer()
        {
            Dispose(false);
        }
    }
}
