using System;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Pixels
{
    public static class PixelMemoryExtensions
    {
        /// <summary>
        /// Gets whether a contiguous span of pixels
        /// can be created over the underlying memory.
        /// </summary>
        public static bool IsPixelContiguous(this IReadOnlyPixelMemory buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return buffer.Size.Width * buffer.ElementSize == buffer.ByteStride;
        }

        public static int GetPixelStride(this IReadOnlyPixelMemory buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (!buffer.IsPixelContiguous())
                throw new InvalidOperationException("The buffer is not pixel-contiguous.");

            return buffer.ByteStride / buffer.ElementSize;
        }

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
