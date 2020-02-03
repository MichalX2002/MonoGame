using System;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only view of pixel byte rows in memory.
    /// </summary>
    public interface IReadOnlyPixelBuffer : IElementContainer, IPixelSource
    { 
        /// <summary>
        /// Gets a row as a span of pixel bytes.
        /// </summary>
        /// <param name="row">The row to get.</param>
        ReadOnlySpan<byte> GetPixelByteRowSpan(int row);
    }

    /// <summary>
    /// Represents a read-only view of  pixel rows in memory.
    /// </summary>
    public interface IReadOnlyPixelBuffer<TPixel> : IReadOnlyPixelBuffer, IPixelSource<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// Gets a row as a span of pixels.
        /// </summary>
        /// <param name="row">The row to get.</param>
        ReadOnlySpan<TPixel> GetPixelRowSpan(int row);
    }
}
