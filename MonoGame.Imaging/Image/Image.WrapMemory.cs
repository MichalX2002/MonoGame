using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static Image WrapMemory(
            VectorTypeInfo pixelType, IMemory memory, Size size, bool leaveOpen = false, int? byteStride = null)
        {
            var wrapDelegate = GetWrapMemoryDelegate(pixelType);
            return wrapDelegate.Invoke(memory, size, leaveOpen, byteStride);
        }

        public static Image<TPixel> WrapMemory<TPixel>(
            IMemory memory, Size size, bool leaveOpen = false, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertDimensionsGreaterThanZero(size, nameof(size));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Count, size.Area, nameof(memory));

            int stride = byteStride ?? (size.Width * Unsafe.SizeOf<TPixel>());
            AssertValidStride<TPixel>(size, memory.Count * memory.ElementSize, stride, nameof(byteStride));

            var buffer = new Image<TPixel>.PixelBuffer(memory, stride, leaveOpen);
            return new Image<TPixel>(buffer, size);
        }

        public static Image<TPixel> WrapMemory<TPixel>(
            Memory<TPixel> memory, Size size, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertDimensionsGreaterThanZero(size, nameof(size));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, size.Area, nameof(memory));

            int elementSize = Unsafe.SizeOf<TPixel>();
            int stride = byteStride ?? (size.Width * elementSize);
            AssertValidStride<TPixel>(size, memory.Length * elementSize, stride, nameof(byteStride));

            var buffer = new Image<TPixel>.PixelBuffer(memory, stride);
            return new Image<TPixel>(buffer, size);
        }

        public static Image<TPixel> WrapMemory<TPixel>(
            Memory<byte> memory, Size size, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertDimensionsGreaterThanZero(size, nameof(size));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, size.Area, nameof(memory));

            int stride = byteStride ?? (size.Width * Unsafe.SizeOf<TPixel>());
            AssertValidStride<TPixel>(size, memory.Length, stride, nameof(byteStride));

            var buffer = new Image<TPixel>.PixelBuffer(memory, stride);
            return new Image<TPixel>(buffer, size);
        }

        [DebuggerHidden]
        private static void AssertValidStride<T>(
            Size size, int memoryByteSize, int byteStride, string paramName)
            where T : unmanaged
        {
            if (size.Width * Unsafe.SizeOf<T>() < byteStride)
                throw new ArgumentOutOfRangeException(
                    "The byte stride is smaller than the byte size of one row.");

            if (memoryByteSize % byteStride != 0)
                throw new ArgumentException(
                    "The byte stride must be aligned with the memory size.", paramName);
        }
    }
}
