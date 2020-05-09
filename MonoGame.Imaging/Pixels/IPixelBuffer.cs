using System;
using MonoGame.Framework.Vector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a view of pixel rows in memory.
    /// </summary>
    public interface IPixelBuffer : IReadOnlyPixelBuffer, IPixelRows
    {
        /// <summary>
        /// Gets a row as a span of pixel bytes.
        /// </summary>
        /// <param name="row">The row to get in pixels.</param>
        new Span<byte> GetPixelByteRowSpan(int row);
    }

    /// <summary>
    /// Represents a view of pixel rows in memory.
    /// </summary>
    public interface IPixelBuffer<TPixel> : IPixelBuffer, IReadOnlyPixelBuffer<TPixel>, IPixelRows<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets a row as a span of pixels.
        /// </summary>
        /// <param name="row">The row to get in pixels.</param>
        new Span<TPixel> GetPixelRowSpan(int row);
    }
}
