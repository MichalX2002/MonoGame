using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Memory
{
    public unsafe class UnmanagedMemory<T> : IMemory<T>, IDisposable
        where T : unmanaged
    {
        private int _length;
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
                return GetByteCount(_length);
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

        [CLSCompliant(false)]
        public ref T Data
        {
            get
            {
                AssertNotDisposed();
                if (Pointer == null)
                    throw new InvalidOperationException(
                        "There is no underlying memory allocated.");
                return ref Unsafe.AsRef<T>((void*)Pointer);
            }
        }

        public IntPtr Pointer { get; private set; }

        public Span<T> Span => new Span<T>((void*)Pointer, _length);
        ref byte IMemory.Data => ref Unsafe.AsRef<byte>((void*)Pointer);

        ReadOnlySpan<T> IReadOnlyMemory<T>.Span => new ReadOnlySpan<T>((void*)Pointer, _length);
        int IReadOnlyMemory.ElementSize => sizeof(T);
        ref readonly byte IReadOnlyMemory.Data => ref Unsafe.AsRef<byte>((void*)Pointer);

        public ref T this[int index]
        {
            get
            {
                T* ptr = (T*)Pointer;
                if (index < 0 || index >= _length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return ref ptr[index];
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the unmanaged pointer with a specified length,
        /// optionally zero-filling the allocated memory.
        /// </summary>
        /// <param name="length">The size in elements.</param>
        /// <param name="zeroFill">
        /// <see langword="true"/> to zero-fill the allocated memory.</param>
        public UnmanagedMemory(int length, bool zeroFill = false)
        {
            ReAlloc(length, zeroFill);
        }

        /// <summary>
        /// Constructs the unmanaged pointer with a null pointer and zero length.
        /// </summary>
        public UnmanagedMemory() : this(0)
        {
        }

        #endregion

        public void Clear()
        {
            Span.Clear();
        }

        public void Fill(T value)
        {
            Span.Fill(value);
        }

        public void Fill(byte value)
        {
            MemoryMarshal.AsBytes(Span).Fill(value);
        }

        #region Helpers

        /// <summary>
        /// Resizes the underlying memory block, 
        /// optionally zero-filling newly allocated memory.
        /// </summary>
        /// <param name="length">The new size in elements. Can be zero to free memory.</param>
        /// <param name="zeroFill"><see langword="true"/> to zero-fill the allocated memory.</param>
        public void ReAlloc(int length, bool zeroFill = false)
        {
            lock (_allocMutex)
            {
                AssertNotDisposed();
                CommonArgumentGuard.AssertAtleastZero(length, nameof(length));
                
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

                        if (Pointer != null)
                            Pointer = Marshal.ReAllocHGlobal(Pointer, (IntPtr)ByteLength);
                        else
                            Pointer = Marshal.AllocHGlobal(ByteLength);

                        if (zeroFill && length > oldLength)
                            Span.Slice(oldLength, length - oldLength).Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Informs the runtime that memory has been released.
        /// </summary>
        /// <param name="oldLength"></param>
        private void ClearPressure(int oldLength)
        {
            if(oldLength != 0)
                GC.RemoveMemoryPressure(GetByteCount(oldLength));
        }

        /// <summary>
        /// Frees pointer, informs the runtime about released memory, and sets length to zero.
        /// </summary>
        /// <param name="oldLength"></param>
        private void FreePtr(int oldLength)
        {
            if (Pointer != null)
            {
                Marshal.FreeHGlobal(Pointer);
                Pointer = default;

                ClearPressure(oldLength);
                _length = 0;
            }
        }
        
        private int GetByteCount(int elementCount)
        {
            return elementCount * sizeof(T);
        }

        #endregion

        #region IDisposable

        [DebuggerHidden]
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(UnmanagedMemory<T>));
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

        ~UnmanagedMemory()
        {
            Dispose(false);
        }

        #endregion
    }
}
