using System;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only view of pixel rows in memory.
    /// </summary>
    public interface IReadOnlyPixelBuffer<TPixel> : IPixelSource<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets a row as a span of pixels.
        /// </summary>
        /// <param name="row">The row to get.</param>
        ReadOnlySpan<TPixel> GetPixelRowSpan(int row);
    }
}
