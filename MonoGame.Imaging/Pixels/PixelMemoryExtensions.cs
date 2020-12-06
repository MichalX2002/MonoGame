using System;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Pixels
{
    public static class PixelMemoryExtensions
    {
        /// <summary>
        /// Gets whether a possibly padded contiguous span of pixels
        /// can be created over the underlying memory.
        /// </summary>
        /// <remarks>
        /// To exclude padding use <see cref="IsPixelContiguous"/>
        /// </remarks>
        public static bool IsPaddedPixelContiguous(this IReadOnlyPixelMemory buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return buffer.ByteStride % buffer.ElementSize == 0;
        }

        /// <summary>
        /// Gets whether a non-padded contiguous span of pixels
        /// can be created over the underlying memory.
        /// </summary>
        /// <remarks>
        /// To include padding use <see cref="IsPaddedPixelContiguous"/>
        /// </remarks>
        public static bool IsPixelContiguous(this IReadOnlyPixelMemory buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return buffer.Width * buffer.ElementSize == buffer.ByteStride;
        }

        public static int GetPixelStride(this IReadOnlyPixelMemory buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (!buffer.IsPaddedPixelContiguous())
                throw new InvalidOperationException("The buffer is not pixel-contiguous.");

            return buffer.ByteStride / buffer.ElementSize;
        }

        public static ReadOnlySpan<TPixel> GetPixelSpan<TPixel>(
            this IReadOnlyPixelMemory<TPixel> buffer, int start)
            where TPixel : unmanaged, IPixel
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return buffer.GetPixelSpan()[start..];
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

            return buffer.GetPixelSpan()[start..];
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
