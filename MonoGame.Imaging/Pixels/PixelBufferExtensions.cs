using System;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Pixels
{
    public static class PixelBufferExtensions
    {
        public static TPixel GetPixel<TPixel>(this IPixelBuffer<TPixel> buffer, int x, int y)
            where TPixel : unmanaged, IPixel
        {
            return buffer.GetPixelRowSpan(y)[x];
        }

        public static void SetPixel<TPixel>(this IPixelBuffer<TPixel> buffer, int x, int y, TPixel value)
            where TPixel : unmanaged, IPixel
        {
            buffer.GetPixelRowSpan(y)[x] = value;
        }

        public static Span<TPixel> GetPixelRow<TPixel>(this IPixelBuffer<TPixel> buffer, int y)
            where TPixel : unmanaged, IPixel
        {
            return buffer.GetPixelRowSpan(y);
        }

        public static Span<TPixel> GetPixelRow<TPixel>(this IPixelBuffer<TPixel> buffer, int x, int y)
            where TPixel : unmanaged, IPixel
        {
            return buffer.GetPixelRow(y).Slice(x);
        }

        public static void GetPixelRow<TPixel>(
            this IPixelBuffer<TPixel> buffer, int x, int y, Span<TPixel> destination)
            where TPixel : unmanaged, IPixel
        {
            buffer.GetPixelRowSpan(y).Slice(x, destination.Length).CopyTo(destination);
        }

        public static void SetPixelRow<TPixel>(
            this IPixelBuffer<TPixel> buffer, int x, int y, Span<TPixel> row)
            where TPixel : unmanaged, IPixel
        {
            row.CopyTo(buffer.GetPixelRowSpan(y).Slice(x));
        }
    }
}
