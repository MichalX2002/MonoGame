using System;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static Image<TPixel> WrapMemory<TPixel>(
            Memory<TPixel> memory, int width, int height, int stride)
            where TPixel : unmanaged, IPixel
        {
            CommonArgumentGuard.AssertAboveZero(width, nameof(width));
            CommonArgumentGuard.AssertAboveZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, width * height, nameof(memory));

            var buffer = new Image<TPixel>.Buffer(memory, stride);
            return new Image<TPixel>(buffer, width, height);
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            Memory<TPixel> memory, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory(memory, width, height, width * sizeof(TPixel));
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            IMemory memory, int width, int height, int pixelStride, bool leaveOpen)
            where TPixel : unmanaged, IPixel
        {
            CommonArgumentGuard.AssertAboveZero(width, nameof(width));
            CommonArgumentGuard.AssertAboveZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, width * height, nameof(memory));

            var buffer = new Image<TPixel>.Buffer(memory, pixelStride, leaveOpen);
            return new Image<TPixel>(buffer, width, height);
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            IMemory memory, int width, int height, bool leaveOpen)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory<TPixel>(memory, width, height, width, leaveOpen);
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            IMemory<TPixel> memory, int width, int height, bool leaveOpen)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory<TPixel>(memory, width, height, leaveOpen);
        }
    }
}
