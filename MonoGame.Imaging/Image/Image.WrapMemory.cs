using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static Image<TPixel> WrapMemory<TPixel>(
            Memory<TPixel> memory, int width, int height, int byteStride)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertGreaterThanZero(width, nameof(width));
            ArgumentGuard.AssertGreaterThanZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, width * height, nameof(memory));

            var buffer = new Image<TPixel>.PixelBuffer(memory, byteStride);
            return new Image<TPixel>(buffer, width, height);
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            Memory<TPixel> memory, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory(memory, width, height, width * sizeof(TPixel));
        }

        public static Image<TPixel> WrapMemory<TPixel>(
            Memory<byte> memory, int width, int height, int byteStride)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertGreaterThanZero(width, nameof(width));
            ArgumentGuard.AssertGreaterThanZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, width * height, nameof(memory));

            var buffer = new Image<TPixel>.PixelBuffer(memory, byteStride);
            return new Image<TPixel>(buffer, width, height);
        }

        public static Image<TPixel> WrapMemory<TPixel>(
            Memory<byte> memory, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory<TPixel>(memory, width, height, width * Unsafe.SizeOf<TPixel>());
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            IMemory memory, int width, int height, int byteStride, bool leaveOpen)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertGreaterThanZero(width, nameof(width));
            ArgumentGuard.AssertGreaterThanZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Count, width * height, nameof(memory));

            var buffer = new Image<TPixel>.PixelBuffer(memory, byteStride, leaveOpen);
            return new Image<TPixel>(buffer, width, height);
        }

        public static Image<TPixel> WrapMemory<TPixel>(
            IMemory memory, int width, int height, bool leaveOpen)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory<TPixel>(memory, width, height, width * Unsafe.SizeOf<TPixel>(), leaveOpen);
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            IMemory<TPixel> memory, int width, int height, bool leaveOpen)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory(memory, width, height, leaveOpen);
        }
    }
}
