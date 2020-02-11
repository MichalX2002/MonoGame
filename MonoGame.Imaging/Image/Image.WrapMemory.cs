using System;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static Image<TPixel> WrapMemory<TPixel>(
            Memory<TPixel> memory, int width, int height, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertGreaterThanZero(width, nameof(width));
            ArgumentGuard.AssertGreaterThanZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, width * height, nameof(memory));

            int stride = byteStride ?? VectorTypeInfo.Get<TPixel>().ElementSize;
            var buffer = new Image<TPixel>.PixelBuffer(memory, stride);
            return new Image<TPixel>(buffer, width, height);
        }

        public static Image<TPixel> WrapMemory<TPixel>(
            Memory<byte> memory, int width, int height, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertGreaterThanZero(width, nameof(width));
            ArgumentGuard.AssertGreaterThanZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, width * height, nameof(memory));
            
            int stride = byteStride ?? VectorTypeInfo.Get<TPixel>().ElementSize;
            var buffer = new Image<TPixel>.PixelBuffer(memory, stride);
            return new Image<TPixel>(buffer, width, height);
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            IMemory memory, bool leaveOpen, int width, int height, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            ArgumentGuard.AssertGreaterThanZero(width, nameof(width));
            ArgumentGuard.AssertGreaterThanZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Count, width * height, nameof(memory));

            int stride = byteStride ?? VectorTypeInfo.Get<TPixel>().ElementSize;
            var buffer = new Image<TPixel>.PixelBuffer(memory, stride, leaveOpen);
            return new Image<TPixel>(buffer, width, height);
        }
    }
}
