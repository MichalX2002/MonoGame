using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static void LoadPixels<TPixelFrom, TPixelTo>(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image<TPixelTo> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            ImagingArgumentGuard.AssertRectangleInSource(pixels, sourceRectangle, nameof(sourceRectangle));

            int byteStride = destination.ByteStride;
            var rowBuffer = byteStride < 4096 ? stackalloc byte[byteStride] : new byte[byteStride];
            var row = MemoryMarshal.Cast<byte, TPixelFrom>(rowBuffer);

            for (int y = 0; y < sourceRectangle.Height; y++)
            {
                pixels.GetPixelByteRow(sourceRectangle.X, sourceRectangle.Y + y, rowBuffer);
                var dstRow = destination.GetPixelRowSpan(y);

                ConvertPixels(row, dstRow);
            }
        }

        public static void LoadPixels(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination)
        {
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            var loadDelegate = GetLoadPixelRowsDelegate(pixels.PixelType, destination.PixelType);
            loadDelegate.Invoke(pixels, sourceRectangle, destination);
        }

        public static Image LoadPixels(
            IReadOnlyPixelRows pixels, VectorType destinationType, Rectangle? sourceRectangle = null)
        {
            if (pixels == null) 
                throw new ArgumentNullException(nameof(pixels));

            var rect = sourceRectangle ?? pixels.GetBounds();
            ImagingArgumentGuard.AssertNonEmptyRectangle(rect, nameof(sourceRectangle));

            var image = Create(destinationType, rect.Size);
            try
            {
                LoadPixels(pixels, rect, image);
            }
            catch
            {
                image.Dispose();
                throw;
            }
            return image;
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            IReadOnlyPixelRows pixels, Rectangle? sourceRectangle = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            if (pixels == null) 
                throw new ArgumentEmptyException(nameof(pixels));
            
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));
            var rect = sourceRectangle ?? pixels.GetBounds();

            var dstImage = new Image<TPixel>(rect.Size);

            if (pixels.PixelType.Type == typeof(TPixel) &&
                pixels is IReadOnlyPixelMemory typeEqualMemory &&
                typeEqualMemory.IsPixelContiguous() &&
                rect.Position == Point.Zero &&
                rect.Size == pixels.Size)
            {
                typeEqualMemory.GetPixelByteSpan().CopyTo(dstImage.GetPixelByteSpan());
            }
            else if (
                pixels.PixelType.Type == typeof(TPixel) &&
                pixels is IReadOnlyPixelBuffer typeEqualBuffer)
            {
                for (int y = 0; y < rect.Height; y++)
                    typeEqualBuffer.GetPixelByteRow(
                        rect.X, rect.Y + y, dstImage.GetPixelByteRowSpan(y));
            }
            else
            {
                LoadPixels(pixels, rect, dstImage);
            }
            return dstImage;
        }
    }
}
