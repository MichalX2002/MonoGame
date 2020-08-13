using MonoGame.Framework;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Pixels
{
    public static class PixelRowsExtensions
    {
        public static Image<TPixel> ToImage<TPixel>(
            this IReadOnlyPixelRows source, Rectangle? sourceRectangle = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return Image.LoadPixels<TPixel>(source, sourceRectangle);
        }

        public static Image<TPixel> ToImage<TPixel>(
            this IReadOnlyPixelRows<TPixel> source, Rectangle? sourceRectangle = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return Image.LoadPixels<TPixel>(source, sourceRectangle);
        }
    }
}
