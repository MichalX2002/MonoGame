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
            IReadOnlyPixelRows pixels, Image<TPixelTo> destination, Rectangle? sourceRectangle = null)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel<TPixelTo>
        {
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            var rect = sourceRectangle ?? pixels.GetBounds();
            ImagingArgumentGuard.AssertNonEmptyRectangle(rect, nameof(sourceRectangle));

            Span<byte> rowByteBuffer = stackalloc byte[400];
            var rowBuffer = MemoryMarshal.Cast<byte, TPixelFrom>(rowByteBuffer);

            for (int y = 0; y < rect.Height; y++)
            {
                var dstRow = destination.GetPixelRowSpan(y);

                int offsetX = 0;
                do
                {
                    int left = rect.Width - offsetX;
                    int count = Math.Min(rowBuffer.Length, left);
                    var slice = rowBuffer.Slice(0, count);

                    pixels.GetPixelByteRow(rect.X + offsetX, rect.Y + y, MemoryMarshal.AsBytes(slice));

                    ConvertPixels(slice, dstRow);
                    dstRow = dstRow.Slice(count);
                    offsetX += count;
                }
                while (offsetX < rect.Width);
            }
        }

        public static void LoadPixels(
            IReadOnlyPixelRows pixels, Image destination, Rectangle? sourceRectangle = null)
        {
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            var loadDelegate = GetLoadPixelRowsDelegate(pixels.PixelType, destination.PixelType);
            loadDelegate.Invoke(pixels, destination, sourceRectangle);
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
                LoadPixels(pixels, image, rect);
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
                LoadPixels(pixels, dstImage, rect);
            }
            return dstImage;
        }
    }
}
