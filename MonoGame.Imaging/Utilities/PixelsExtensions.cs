using System;
using System.Diagnostics.CodeAnalysis;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public static class PixelBufferExtensions
    {
        /// <summary>
        /// Gets the width and height of the view as a <see cref="Size"/>.
        /// </summary>
        /// <param name="view"></param>
        /// <returns>The buffer size in pixels.</returns>
        public static Size GetSize<TPixel>(this IPixelSource<TPixel> source)
            where TPixel : unmanaged, IPixel
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new Size(source.Width, source.Height);
        }

        /// <summary>
        /// Gets the width and height of the view as a <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="view"></param>
        /// <returns>The buffer bounds starting at point [0, 0].</returns>
        public static Rectangle GetBounds<TPixel>(this IPixelSource<TPixel> source)
            where TPixel : unmanaged, IPixel
        {
            return new Rectangle(Point.Zero, GetSize(source));
        }

        /// <summary>
        /// Gets the size of type <typeparamref name="TPixel"/> in bits.
        /// </summary>
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension method")]
        public static unsafe int GetBitDepth<TPixel>(this IPixelSource<TPixel> view)
            where TPixel : unmanaged, IPixel
        {
            return sizeof(TPixel) * 8;
        }

        /// <summary>
        /// Gets whether the pixel store has extra padding at the end of each row.
        /// </summary>
        public static unsafe bool HasPadding<TPixel>(this IReadOnlyPixelMemory<TPixel> memory)
            where TPixel : unmanaged, IPixel
        {
            if (memory == null) throw new ArgumentNullException(nameof(memory));
            return memory.Stride != memory.Width;
        }

        /// <summary>
        /// Gets the data stride (row width) including padding of the view in bytes.
        /// </summary>
        public static unsafe int GetByteStride<TPixel>(this IReadOnlyPixelMemory<TPixel> memory)
            where TPixel : unmanaged, IPixel
        {
            if (memory == null) throw new ArgumentNullException(nameof(memory));
            return memory.Stride * sizeof(TPixel);
        }

        #region GetPixelSpan

        public static ReadOnlySpan<TPixel> GetPixelSpan<TPixel>(
            this IReadOnlyPixelMemory<TPixel> buffer, int start)
            where TPixel : unmanaged, IPixel
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            return buffer.GetPixelSpan().Slice(start);
        }

        public static ReadOnlySpan<TPixel> GetPixelSpan<TPixel>(
            this IReadOnlyPixelMemory<TPixel> buffer, int start, int length)
            where TPixel : unmanaged, IPixel
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            return buffer.GetPixelSpan().Slice(start, length);
        }

        public static Span<TPixel> GetPixelSpan<TPixel>(
           this IPixelMemory<TPixel> buffer, int start)
           where TPixel : unmanaged, IPixel
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            return buffer.GetPixelSpan().Slice(start);
        }

        public static Span<TPixel> GetPixelSpan<TPixel>(
            this IPixelMemory<TPixel> buffer, int start, int length)
            where TPixel : unmanaged, IPixel
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            return buffer.GetPixelSpan().Slice(start, length);
        }

        #endregion
    }
}
