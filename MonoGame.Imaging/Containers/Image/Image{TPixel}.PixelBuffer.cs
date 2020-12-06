using System;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vectors;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MonoGame.Imaging
{
    public partial class Image<TPixel> : IPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        public override Span<byte> GetPixelByteSpan()
        {
            return Buffer.ByteSpan;
        }

        public override Span<byte> GetPixelByteRowSpan(int row)
        {
            if ((uint)row >= Height)
                throw new ArgumentOutOfRangeException(nameof(row), "The requested row is out of bounds.");

            return Buffer.ByteSpan.Slice(row * Buffer.ByteStride, Width * PixelType.ElementSize);
        }

        public Span<TPixel> GetPixelSpan()
        {
            if (!this.IsPaddedPixelContiguous())
                throw new InvalidOperationException("The underlying memory is not pixel contiguous.");

            return Buffer.PixelSpan;
        }

        ReadOnlySpan<TPixel> IReadOnlyPixelMemory<TPixel>.GetPixelSpan()
        {
            return GetPixelSpan();
        }

        public Span<TPixel> GetPixelRowSpan(int row)
        {
            return MemoryMarshal.Cast<byte, TPixel>(GetPixelByteRowSpan(row));
        }

        ReadOnlySpan<TPixel> IReadOnlyPixelBuffer<TPixel>.GetPixelRowSpan(int row)
        {
            return GetPixelRowSpan(row);
        }

        public void GetPixelRow(int x, int y, Span<TPixel> destination)
        {
            var rowSpan = GetPixelRowSpan(y);
            rowSpan[x..].CopyTo(destination);
        }

        public void SetPixelRow(int x, int y, ReadOnlySpan<TPixel> data)
        {
            var rowSpan = GetPixelRowSpan(y);
            data.CopyTo(rowSpan[x..]);
        }

        public void SetPixelColumn(int x, int y, ReadOnlySpan<TPixel> data)
        {
            int rows = data.Length / PixelType.ElementSize;
            for (int srcRow = 0; srcRow < rows; srcRow++)
            {
                var dstSlice = GetPixelRowSpan(srcRow + y);
                dstSlice[x] = data[srcRow];
            }
        }

        /// <summary>
        /// Helper for storing and accessing image pixels from different memory sources.
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

            public Span<TPixel> PixelSpan
            {
                get
                {
                    if (_imemory != null)
                        return MemoryMarshal.Cast<byte, TPixel>(_imemory.ByteSpan);
                    if (!_bmemory.IsEmpty)
                        return MemoryMarshal.Cast<byte, TPixel>(_bmemory.Span);
                    return _memory.Span;
                }
            }

            public Span<byte> ByteSpan
            {
                get
                {
                    if (_imemory != null)
                        return _imemory.ByteSpan;
                    if (!_bmemory.IsEmpty)
                        return _bmemory.Span;
                    return MemoryMarshal.Cast<TPixel, byte>(_memory.Span);
                }
            }

            public PixelBuffer(IMemory memory, int byteStride, bool leaveOpen) : this()
            {
                _imemory = memory ?? throw new ArgumentNullException(nameof(memory));
                _leaveOpen = leaveOpen;
                ElementCount = memory.Length;
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
