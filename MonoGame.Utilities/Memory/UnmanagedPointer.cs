using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MonoGame.Utilities.Memory
{
    public unsafe class UnmanagedPointer<T> : IDisposable
        where T : unmanaged
    {
        private int _length;
        private IntPtr _safePtr;
        private object _allocMutex = new object();

        #region Properties + Indexer

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the length of the pointer in bytes.
        /// </summary>
        public int ByteLength
        {
            get
            {
                AssertNotDisposed();
                return GetBytes(_length);
            }
        }
        
        /// <summary>
        /// Gets or sets the length of the pointer in elements.
        /// </summary>
        public int Length
        {
            get
            {
                AssertNotDisposed();
                return _length;
            }
            set => ReAlloc(value);
        }

        public IntPtr SafePtr
        {
            get
            {
                AssertNotDisposed();
                return _safePtr;
            }
        }

        public T* Ptr
        {
            get
            {
                AssertNotDisposed();
                return (T*)_safePtr;
            }
        }

        public Span<T> Span => new Span<T>(Ptr, _length);

        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index >= _length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return ref Ptr[index];
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the unmanaged pointer with a specified length.
        /// </summary>
        /// <param name="length">The size in elements.</param>
        public UnmanagedPointer(int length)
        {
            ReAlloc(length);
        }

        /// <summary>
        /// Constructs the unmanaged pointer with a null pointer and zero length.
        /// </summary>
        public UnmanagedPointer() : this(0)
        {
        }

        #endregion

        #region Helpers

        private void ReAlloc(int length)
        {
            lock (_allocMutex)
            {
                AssertNotDisposed();

                if (length < 0)
                    throw new ArgumentOutOfRangeException("Value must be above zero.", nameof(length));

                if (_length != length)
                {
                    int oldLength = _length;
                    _length = length;

                    if (length == 0)
                    {
                        FreePtr(oldLength);
                    }
                    else
                    {
                        ClearPressure(oldLength);
                        GC.AddMemoryPressure(ByteLength);

                        if (_safePtr != IntPtr.Zero)
                            _safePtr = Marshal.ReAllocHGlobal(_safePtr, (IntPtr)ByteLength);
                        else
                            _safePtr = Marshal.AllocHGlobal(ByteLength);
                    }
                }
            }
        }

        /// <summary>
        /// Clears GC memory pressure.
        /// </summary>
        /// <param name="oldLength"></param>
        private void ClearPressure(int oldLength)
        {
            if(oldLength != 0)
                GC.RemoveMemoryPressure(GetBytes(oldLength));
        }

        /// <summary>
        /// Frees pointer, clears GC memory pressure, and sets length to zero.
        /// </summary>
        /// <param name="oldLength"></param>
        private void FreePtr(int oldLength)
        {
            if (_safePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_safePtr);
                _safePtr = IntPtr.Zero;

                ClearPressure(oldLength);
                _length = 0;
            }
        }
        
        private int GetBytes(int elementCount)
        {
            return elementCount * sizeof(T);
        }

        #endregion

        #region IDisposable

        [DebuggerHidden]
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(UnmanagedPointer<T>));
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_allocMutex)
            {
                if (!IsDisposed)
                {
                    FreePtr(_length);
                    IsDisposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnmanagedPointer()
        {
            Dispose(false);
        }

        #endregion
    }
}
