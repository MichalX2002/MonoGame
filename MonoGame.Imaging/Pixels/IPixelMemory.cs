using System;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Pixels
{
    public interface IPixelMemory : IReadOnlyPixelMemory, IPixelBuffer
    {
        /// <summary>
        /// Gets the underlying memory as a contigous span of pixel bytes.
        /// </summary>
        new Span<byte> GetPixelByteSpan();
    }

    /// <summary>
    /// Represents a view of pixels in contiguous memory.
    /// </summary>
    public interface IPixelMemory<TPixel> : IPixelMemory, IReadOnlyPixelMemory<TPixel>, IPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the underlying memory as a contigous span of pixels.
        /// </summary>
        new Span<TPixel> GetPixelSpan();
    }
}
