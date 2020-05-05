using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        private static void LoadPixelsCore<TPixelFrom, TPixelTo>(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image<TPixelTo> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            LoadPixels<TPixelFrom, TPixelTo>(pixels, sourceRectangle, destination);
        }

        public static void LoadPixels<TPixelFrom, TPixelTo>(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image<TPixelTo> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            // TODO: use stack-allocated/ArrayPool buffer
            var rowBuffer = new TPixelFrom[destination.Width];
            var rowBufferBytes = MemoryMarshal.AsBytes(rowBuffer.AsSpan());

            for (int y = 0; y < sourceRectangle.Height; y++)
            {
                pixels.GetPixelByteRow(sourceRectangle.X, sourceRectangle.Y + y, rowBufferBytes);

                var dstRow = destination.GetPixelRowSpan(y);
                for (int x = 0; x < sourceRectangle.Width; x++)
                {
                    rowBuffer[x].ToScaledVector4(out var vector);
                    dstRow[x].FromScaledVector4(vector);
                }
            }
        }

        public static void LoadPixels(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination)
        {
            var loadDelegate = GetLoadPixelRowsDelegate(pixels.PixelType, destination.PixelType);
            loadDelegate.Invoke(pixels, sourceRectangle, destination);
        }

        public static Image LoadPixels(
            IReadOnlyPixelRows pixels, VectorTypeInfo destinationType, Rectangle? sourceRectangle = null)
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));

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
            where TPixel : unmanaged, IPixel
        {
            // TODO: benchmark; replace Span<>.CopyTo with possibly faster memcpy

            if (pixels == null) throw new ArgumentEmptyException(nameof(pixels));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));
            var rect = sourceRectangle ?? pixels.GetBounds();

            var dstImage = new Image<TPixel>(rect.Size);

            if (pixels.PixelType.Type == typeof(TPixel) &&
                pixels is IReadOnlyPixelMemory typeEqualMemory &&
                typeEqualMemory.IsPixelContiguous &&
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
