using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework
{
    public unsafe class UnmanagedPointer<T> : IDisposable
        where T : unmanaged
    {
        private int _length;
        private IntPtr _ptr;

        public bool IsDisposed { get; private set; }
        public IntPtr SafePtr => _ptr;
        public T* Ptr => (T*)_ptr;
        public int Bytes => _length * sizeof(T);
        public int Length { get => _length; set => ReAlloc(value); }

        public T this[int index]
        {
            get => Ptr[index];
            set => Set(index, value);
        }

        public UnmanagedPointer(int length)
        {
            ReAlloc(length);
        }

        public UnmanagedPointer() : this(0)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, in T value)
        {
            Ptr[index] = value;
        }

        private void ReAlloc(int length)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(UnmanagedPointer<T>));

            if (length < 0)
                throw new ArgumentOutOfRangeException(
                    "Must be above zero.", nameof(length));

            if (_length != length)
            {
                _length = length;
                if (_length == 0)
                {
                    FreePtr();
                    return;
                }

                if (_ptr != IntPtr.Zero)
                    _ptr = Marshal.ReAllocHGlobal(_ptr, (IntPtr)Bytes);
                else
                    Marshal.AllocHGlobal(Bytes);
            }
        }

        private void FreePtr()
        {
            if (_ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_ptr);
                _ptr = IntPtr.Zero;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                FreePtr();
                IsDisposed = true;
            }
        }

        ~UnmanagedPointer()
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
