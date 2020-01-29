using System;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only view of pixels in contiguous memory.
    /// </summary>
    public interface IReadOnlyPixelMemory<TPixel> : IReadOnlyPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the data stride (row width including padding) of the view in bytes.
        /// </summary>
        int Stride { get; }

        /// <summary>
        /// Gets the underlying memory as a contigous span of pixels.
        /// </summary>
        ReadOnlySpan<TPixel> GetPixelSpan();
    }
}
