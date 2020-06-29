using System;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only view of pixel byte rows in memory.
    /// </summary>
    public interface IReadOnlyPixelBuffer : IReadOnlyPixelRows
    { 
        /// <summary>
        /// Gets a row as a span of pixel bytes.
        /// </summary>
        /// <param name="row">The row to get in pixels.</param>
        ReadOnlySpan<byte> GetPixelByteRowSpan(int row);
    }

    /// <summary>
    /// Represents a read-only view of pixel rows in memory.
    /// </summary>
    public interface IReadOnlyPixelBuffer<TPixel> : IReadOnlyPixelBuffer, IReadOnlyPixelRows<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// Gets a row as a span of pixels.
        /// </summary>
        /// <param name="row">The row to get in pixels.</param>
        ReadOnlySpan<TPixel> GetPixelRowSpan(int row);
    }
}
