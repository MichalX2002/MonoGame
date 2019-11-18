using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public static partial class Image
    {
        #region LoadPixels(ReadOnlySpan<byte>)

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int stride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixelData.IsEmpty) throw new ArgumentEmptyException(nameof(pixelData));
            if (config == null) throw new ArgumentNullException(nameof(config));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixel>(sourceRectangle.Width, sourceRectangle.Height);
            fixed (byte* srcPixelPtr = &MemoryMarshal.GetReference(pixelData))
            fixed (TPixel* dstPixelPtr = &MemoryMarshal.GetReference(image.GetPixelSpan()))
            {
                byte* srcPtr = srcPixelPtr + sourceRectangle.X;
                byte* dstPtr = (byte*)(dstPixelPtr + sourceRectangle.X);
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    int rowOffset = (sourceRectangle.Y + y) * stride;
                    byte* srcRow = srcPtr + rowOffset;
                    byte* dstRow = dstPtr + rowOffset;
                    Buffer.MemoryCopy(srcRow, dstRow, stride, stride);
                }
            }
            return image;
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<byte> pixelData, Rectangle sourceRectangle, int stride)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixels<TPixel>(pixelData, sourceRectangle, stride, ImagingConfig.Default);
        }

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            var bytes = MemoryMarshal.AsBytes(pixelData);
            var srcRect = new Rectangle(0, 0, width, height);
            return LoadPixels<TPixel>(bytes, srcRect, width * sizeof(TPixel), ImagingConfig.Default);
        }

        #endregion

        #region LoadPixels(ReadOnlySpan<TPixel>, sourceRectangle)

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, Rectangle sourceRectangle, int stride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            var bytes = MemoryMarshal.AsBytes(pixelData);
            return LoadPixels<TPixel>(bytes, sourceRectangle, stride, config);
        }

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, Rectangle sourceRectangle, int stride)
            where TPixel : unmanaged, IPixel => 
            LoadPixels(pixelData, sourceRectangle, stride, ImagingConfig.Default);

        #endregion

        #region LoadPixels(ReadOnlySpan<TPixel>, width, height)

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, int width, int height, int stride, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            var srcRect = new Rectangle(0, 0, width, height);
            return LoadPixels(pixelData, srcRect, stride, config);
        }

        public static unsafe Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixelData, int width, int height, int stride)
            where TPixel : unmanaged, IPixel => 
            LoadPixels(pixelData, width, height, stride, ImagingConfig.Default);

        #endregion
    }
}
