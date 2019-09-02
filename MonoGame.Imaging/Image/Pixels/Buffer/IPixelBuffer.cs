using System;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a view of pixels in memory.
    /// </summary>
    /// <typeparam name="TPixel"></typeparam>
    public interface IPixelBuffer<TPixel> : IReadOnlyPixelBuffer<TPixel>, IPixelRows<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets a row as a span of pixels.
        /// </summary>
        /// <param name="row">The row to get.</param>
        new Span<TPixel> GetPixelRowSpan(int row);
    }
}
