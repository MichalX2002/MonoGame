using System;
using System.Diagnostics;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using System.Runtime.CompilerServices;

namespace MonoGame.Imaging
{
    public partial class Image<TPixel> : IPixelMemory<TPixel>, IDisposable
        where TPixel : unmanaged, IPixel
    {
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

        public Span<TPixel> GetPixelRowSpan(int row)
        {
            AssertInRange(row, nameof(row));
            return this.GetPixelSpan(row * _pixelBuffer.PixelStride, Width);
        }

        ReadOnlySpan<TPixel> IReadOnlyPixelBuffer<TPixel>.GetPixelRowSpan(int row)
        {
            AssertInRange(row, nameof(row));
            return this.GetPixelSpan(row * _pixelBuffer.PixelStride, Width);
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
        /// Helper for containing image pixels in memory.
        /// </summary>
        public struct Buffer
        {
            private IMemory _imemory;
            private Memory<TPixel> _memory;
            private bool _leaveOpen;

            public int Length { get; }
            public int PixelStride { get; }
            public bool IsEmpty => Length == 0;

            public unsafe Span<TPixel> Span
            {
                get
                {
                    if (_imemory != null)
                        return new Span<TPixel>(Unsafe.AsPointer(ref _imemory.Data), Length);
                    return _memory.Span;
                }
            }

            public Buffer(IMemory memory, int pixelStride, bool leaveOpen) : this()
            {
                _imemory = memory ?? throw new ArgumentNullException(nameof(memory));
                _leaveOpen = leaveOpen;
                Length = memory.Length;
                PixelStride = pixelStride;
            }

            public Buffer(Memory<TPixel> memory, int pixelStride) : this()
            {
                if (memory.IsEmpty)
                    throw new ArgumentEmptyException(nameof(memory));

                _memory = memory;
                Length = memory.Length;
                PixelStride = pixelStride;
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
