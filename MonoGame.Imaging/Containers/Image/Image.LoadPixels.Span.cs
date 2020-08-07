using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static void LoadPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> pixels,
            Rectangle sourceRectangle,
            Image<TPixelTo> destination,
            int? pixelStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            int srcStride = pixelStride ?? sourceRectangle.Width;

            int dstByteStride = destination.ByteStride;
            var dstBytes = destination.GetPixelByteSpan();

            for (int y = 0; y < sourceRectangle.Height; y++)
            {
                var srcRow = pixels.Slice(sourceRectangle.X + (sourceRectangle.Y + y) * srcStride, srcStride);
                var dstByteRow = dstBytes.Slice(y * dstByteStride, dstByteStride);

                var dstRow = MemoryMarshal.Cast<byte, TPixelTo>(dstByteRow);
                ConvertPixels(srcRow, dstRow);
            }
        }

        #region LoadPixels(ReadOnlySpan)

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> pixels,
            Rectangle sourceRectangle,
            int? pixelStride = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            if (pixels.IsEmpty) 
                throw new ArgumentEmptyException(nameof(pixels));

            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixelTo>(sourceRectangle.Size);
            try
            {
                LoadPixels(pixels, sourceRectangle, image, pixelStride);
            }
            catch
            {
                image.Dispose();
                throw;
            }
            return image;
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            ReadOnlySpan<TPixel> pixels, Rectangle sourceRectangle, int? pixelStride = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return LoadPixels<TPixel, TPixel>(pixels, sourceRectangle, pixelStride);
        }

        #endregion

        #region LoadPixels(Span)

        public static Image<TPixelTo> LoadPixels<TPixelFrom, TPixelTo>(
            Span<TPixelFrom> pixels, Rectangle sourceRectangle, int? pixelStride)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            return LoadPixels<TPixelFrom, TPixelTo>((ReadOnlySpan<TPixelFrom>)pixels, sourceRectangle, pixelStride);
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            Span<TPixel> pixels, Rectangle sourceRectangle, int? pixelStride)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return LoadPixels<TPixel, TPixel>((ReadOnlySpan<TPixel>)pixels, sourceRectangle, pixelStride);
        }

        #endregion
    }
}
