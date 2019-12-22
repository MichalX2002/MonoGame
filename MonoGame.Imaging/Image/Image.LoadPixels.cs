using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public static partial class Image
    {
        #region LoadPixels(ReadOnlySpan<byte>)

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixelData.IsEmpty) throw new ArgumentEmptyException(nameof(pixelData));
            if (config == null) throw new ArgumentNullException(nameof(config));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixel>(sourceRectangle.Width, sourceRectangle.Height);
            int dstStride = image.GetByteStride();

            int srcRowBytes = sizeof(TPixel) * sourceRectangle.Width;
            int srcStride = byteStride ?? srcRowBytes;
            
            // TODO: check if the pixelData isn't padded and copy everything at once

            fixed (byte* srcPixelPtr = &MemoryMarshal.GetReference(pixelData))
            fixed (TPixel* dstPixelPtr = &MemoryMarshal.GetReference(image.GetPixelSpan()))
            {
                byte* srcPtr = srcPixelPtr + sourceRectangle.X;
                byte* dstPtr = (byte*)(dstPixelPtr + sourceRectangle.X);
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    byte* srcRow = srcPtr + (sourceRectangle.Y + y) * srcStride;
                    byte* dstRow = dstPtr + (sourceRectangle.Y + y) * dstStride;
                    Buffer.MemoryCopy(srcRow, dstRow, dstStride, srcRowBytes);
                }
            }
            return image;
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel>(pixelData, sourceRectangle, byteStride, ImagingConfig.Default);
        }

        #endregion

        #region LoadPixels(ReadOnlySpan<TPixel>, sourceRectangle)

        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, Rectangle sourceRectangle, int? byteStride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            var bytes = MemoryMarshal.AsBytes(pixelData);
            return LoadPixels<TPixel>(bytes, sourceRectangle, byteStride, config);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, Rectangle sourceRectangle, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels(pixelData, sourceRectangle, byteStride, ImagingConfig.Default);
        }

        #endregion

        #region LoadPixels(ReadOnlySpan<TPixel>, width, height)

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, int width, int height, int? byteStride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            var srcRect = new Rectangle(0, 0, width, height);
            return LoadPixels(pixelData, srcRect, byteStride, config);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, int width, int height, int? byteStride = null)
            where TPixel : unmanaged, IPixel
        {
            var bytes = MemoryMarshal.AsBytes(pixelData);
            var srcRect = new Rectangle(0, 0, width, height);
            return LoadPixels<TPixel>(bytes, srcRect, byteStride, ImagingConfig.Default);
        }

        #endregion
    }
}
