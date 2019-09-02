using System;
using System.Diagnostics;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public unsafe partial class Image<TPixel> : IPixelBuffer<TPixel>, IDisposable
        where TPixel : unmanaged, IPixel
    {
        public TPixel GetPixel(int x, int y)
        {
            return GetPixelRowSpan(y)[x];
        }

        public void SetPixel(int x, int y, TPixel value)
        {
            GetPixelRowSpan(y)[x] = value;
        }

        public void GetPixelRow(int x, int y, Span<TPixel> destination)
        {
            GetPixelRowSpan(y).Slice(x, destination.Length).CopyTo(destination);
        }

        public void SetPixelRow(int x, int y, Span<TPixel> row)
        {
            row.CopyTo(GetPixelRowSpan(y).Slice(x));
        }

        public Span<TPixel> GetPixelSpan()
        {
            AssertNotDisposed();
            return _pixelBuffer.Span;
        }

        ReadOnlySpan<TPixel> IReadOnlyPixelMemory<TPixel>.GetPixelSpan()
        {
            AssertNotDisposed();
            return _pixelBuffer.Span;
        }

        public unsafe Span<TPixel> GetPixelRowSpan(int row)
        {
            AssertInRange(row, nameof(row));
            return this.GetPixelSpan(row * Stride, Width);
        }

        unsafe ReadOnlySpan<TPixel> IReadOnlyPixelBuffer<TPixel>.GetPixelRowSpan(int row)
        {
            AssertInRange(row, nameof(row));
            return this.GetPixelSpan(row * Stride, Width);
        }

        [DebuggerHidden]
        private void AssertInRange(int row, string paramName)
        {
            CommonArgumentGuard.AssertAtleastZero(row, paramName);
            if (row >= Height)
                throw new ArgumentOutOfRangeException(
                    paramName, "The requested row is out of bounds.");
        }

        /// <summary>
        /// Helper to keep the main class tidy.
        /// </summary>
        internal unsafe struct Buffer
        {
            private IMemory<TPixel> _imemory;
            private Memory<TPixel> _memory;
            private bool _leaveOpen;

            public int Length { get; }
            public int Stride { get; }
            public bool IsEmpty => Length == 0;

            public Span<TPixel> Span
            {
                get
                {
                    if (_imemory != null)
                        return _imemory.Span;
                    return _memory.Span;
                }
            }

            public Buffer(IMemory<TPixel> memory, int stride, bool leaveOpen) : this()
            {
                _imemory = memory ?? throw new ArgumentNullException(nameof(memory));
                _leaveOpen = leaveOpen;
                Length = memory.Span.Length;
                Stride = stride;
            }

            public Buffer(Memory<TPixel> memory, int stride) : this()
            {
                if (memory.IsEmpty)
                    throw new ArgumentEmptyException(nameof(memory));

                _memory = memory;
                Length = memory.Length;
                Stride = stride;
            }

            public void Dispose()
            {
                if (!_leaveOpen)
                    _imemory?.Dispose();
                this = default;
            }
        }
    }
}
