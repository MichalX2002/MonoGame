using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework
{
    public unsafe class UnmanagedPointer<T> : IDisposable
        where T : unmanaged
    {
        private int _length;

        public bool IsDisposed { get; private set; }
        public IntPtr SafePtr { get; private set; }
        public T* Ptr => (T*)SafePtr;
        public int Bytes => _length * sizeof(T);
        public int Length { get => _length; set => ReAlloc(value); }

        public T this[int index]
        {
            get => Ptr[index];
            set => Set(index, ref value);
        }

        public UnmanagedPointer(int length)
        {
            ReAlloc(length);
        }

        public UnmanagedPointer() : this(0)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, ref T value)
        {
            if (index >= _length)
                throw new ArgumentOutOfRangeException(nameof(index));
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

                if (SafePtr != IntPtr.Zero)
                    SafePtr = Marshal.ReAllocHGlobal(SafePtr, (IntPtr)Bytes);
                else
                    SafePtr = Marshal.AllocHGlobal(Bytes);
            }
        }

        private void FreePtr()
        {
            if (SafePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(SafePtr);
                SafePtr = IntPtr.Zero;
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
