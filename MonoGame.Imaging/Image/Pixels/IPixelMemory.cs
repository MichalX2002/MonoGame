using System;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a view of pixels in contiguous memory.
    /// </summary>
    public interface IPixelMemory<TPixel> : IReadOnlyPixelMemory<TPixel>, IPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the underlying memory as a contigous span of pixels.
        /// </summary>
        new Span<TPixel> GetPixelSpan();
    }
}
