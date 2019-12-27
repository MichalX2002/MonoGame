using System;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a view of pixel rows in memory.
    /// </summary>
    /// <typeparam name="TPixel"></typeparam>
    public interface IPixelBuffer<TPixel> : IReadOnlyPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets a row as a span of pixels.
        /// </summary>
        /// <param name="row">The row to get.</param>
        new Span<TPixel> GetPixelRowSpan(int row);
    }
}
