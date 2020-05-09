using System;
using MonoGame.Framework.Vector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public static class PixelMemoryExtensions
    {
        public static ReadOnlySpan<TPixel> GetPixelSpan<TPixel>(
            this IReadOnlyPixelMemory<TPixel> buffer, int start)
            where TPixel : unmanaged, IPixel
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            return buffer.GetPixelSpan().Slice(start);
        }

        public static ReadOnlySpan<TPixel> GetPixelSpan<TPixel>(
            this IReadOnlyPixelMemory<TPixel> buffer, int start, int length)
            where TPixel : unmanaged, IPixel
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            return buffer.GetPixelSpan().Slice(start, length);
        }

        public static Span<TPixel> GetPixelSpan<TPixel>(
           this IPixelMemory<TPixel> buffer, int start)
           where TPixel : unmanaged, IPixel
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            return buffer.GetPixelSpan().Slice(start);
        }

        public static Span<TPixel> GetPixelSpan<TPixel>(
            this IPixelMemory<TPixel> buffer, int start, int length)
            where TPixel : unmanaged, IPixel
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            return buffer.GetPixelSpan().Slice(start, length);
        }
    }
}
