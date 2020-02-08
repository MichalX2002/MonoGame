using System;
using System.Collections.Concurrent;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        private delegate void LoadPixelRowsDelegate(
            IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination);

        private static ConcurrentDictionary<(Type, Type), LoadPixelRowsDelegate> _loadPixelRowsDelegateCache =
            new ConcurrentDictionary<(Type, Type), LoadPixelRowsDelegate>();

        public static Image LoadPixels(
            PixelTypeInfo imageType, IReadOnlyPixelRows pixels, Rectangle sourceRectangle)
        {
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));
            var image = Create(sourceRectangle.Size, imageType);

        }

        private static void LoadPixels(
            PixelTypeInfo imageType, IReadOnlyPixelRows pixels, Rectangle sourceRectangle, Image destination)
        {
        }

        public static Image<TPixel> LoadPixels<TPixel>(
            IReadOnlyPixelRows pixels, Rectangle? sourceRectangle = null)
            where TPixel : unmanaged, IPixel
        {
            // TODO: test optimization: replacing Span<>.CopyTo with possibly faster memcpy

            if (pixels == null) throw new ArgumentEmptyException(nameof(pixels));
            var rect = sourceRectangle ?? pixels.GetBounds();
            ImagingArgumentGuard.AssertNonEmptyRectangle(rect, nameof(rect));

            var dstImage = new Image<TPixel>(rect.Width, rect.Height);
            if (pixels is IReadOnlyPixelMemory<TPixel> typeEqualMemory)
            {
                if (typeEqualMemory.IsPixelContiguous &&
                    rect.Position == Point.Zero &&
                    rect.Width == pixels.Width &&
                    rect.Height == pixels.Height)
                {
                    typeEqualMemory.GetPixelSpan().CopyTo(dstImage.GetPixelSpan());
                }
                else
                {
                    for (int y = 0; y < rect.Height; y++)
                        typeEqualMemory.GetPixelRow(
                            rect.X, rect.Y + y, dstImage.GetPixelRowSpan(y));
                }
            }
            else if (pixels is IReadOnlyPixelBuffer<TPixel> typeEqualBuffer)
            {
                for (int y = 0; y < rect.Height; y++)
                    typeEqualBuffer.GetPixelRow(
                        rect.X, rect.Y + y, dstImage.GetPixelRowSpan(y));
            }
            else
            {
                // TODO: make stack-allocated buffer
                var rowBuffer = new TPixelFrom[dstImage.Width];

                for (int y = 0; y < rect.Height; y++)
                {
                    pixels.GetPixelByteRow(rect.X, rect.Y + y, rowBuffer);
                    var dstRow = dstImage.GetPixelRowSpan(y);
                    for (int x = 0; x < rect.Width; x++)
                        dstRow[x].FromScaledVector4(rowBuffer[x].ToScaledVector4());
                }
            }
            return dstImage;
        }
    }
}
