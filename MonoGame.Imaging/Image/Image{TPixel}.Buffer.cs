using System;
using System.Diagnostics;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MonoGame.Imaging
{
    public partial class Image<TPixel> : IPixelBuffer<TPixel>, IDisposable
        where TPixel : unmanaged, IPixel
    {
        //public Span<TPixel> GetPixelSpan()
        //{
        //    AssertNotDisposed();
        //    return _pixelBuffer.Span;
        //}
        //
        //ReadOnlySpan<TPixel> IReadOnlyPixelMemory<TPixel>.GetPixelSpan()
        //{
        //    AssertNotDisposed();
        //    return _pixelBuffer.Span;
        //}

        public Span<TPixel> GetPixelRowSpan(int row)
        {
            AssertInRange(row, nameof(row));
            return this.GetPixelSpan(row * Buffer.ByteStride, Width);
        }

        ReadOnlySpan<TPixel> IReadOnlyPixelBuffer<TPixel>.GetPixelRowSpan(int row)
        {
            AssertInRange(row, nameof(row));
            return this.GetPixelSpan(row * Buffer.ByteStride, Width);
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
        public struct PixelBuffer
        {
            private IMemory _imemory;
            private Memory<byte> _bmemory;
            private Memory<TPixel> _memory;
            private bool _leaveOpen;

            public int ElementCount { get; }
            public int ByteStride { get; }
            public bool IsEmpty => ElementCount == 0;

            public Span<TPixel> Span
            {
                get
                {
                    if (_imemory != null)
                        return MemoryMarshal.Cast<byte, TPixel>(_imemory.Span);
                    if (!_bmemory.IsEmpty)
                        return MemoryMarshal.Cast<byte, TPixel>(_bmemory.Span);
                    return _memory.Span;
                }
            }

            public PixelBuffer(IMemory memory, int byteStride, bool leaveOpen) : this()
            {
                _imemory = memory ?? throw new ArgumentNullException(nameof(memory));
                _leaveOpen = leaveOpen;
                ElementCount = memory.Count;
                ByteStride = byteStride;
            }

            public PixelBuffer(Memory<TPixel> memory, int byteStride) : this()
            {
                if (memory.IsEmpty)
                    throw new ArgumentEmptyException(nameof(memory));

                _memory = memory;
                ElementCount = memory.Length;
                ByteStride = byteStride;
            }

            public PixelBuffer(Memory<byte> memory, int byteStride) : this()
            {
                if (memory.IsEmpty)
                    throw new ArgumentEmptyException(nameof(memory));

                _bmemory = memory;
                ElementCount = memory.Length / Unsafe.SizeOf<TPixel>();
                ByteStride = byteStride;
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
