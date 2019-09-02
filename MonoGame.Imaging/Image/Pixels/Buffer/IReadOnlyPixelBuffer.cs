using System;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only view of pixels in memory.
    /// </summary>
    public interface IReadOnlyPixelBuffer<TPixel> : IReadOnlyPixelView<TPixel>, IReadOnlyPixelRows<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets a row as a span of pixels.
        /// </summary>
        /// <param name="row">The row to get.</param>
        ReadOnlySpan<TPixel> GetPixelRowSpan(int row);
    }
}
