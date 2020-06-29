using System;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only view of pixels in contiguous memory.
    /// </summary>
    public interface IReadOnlyPixelMemory : IReadOnlyPixelBuffer
    {
        /// <summary>
        /// Gets the data stride (row width including padding) of the source in bytes.
        /// </summary>
        int ByteStride { get; }

        /// <summary>
        /// Gets whether a contiguous span of pixels
        /// can be created over the underlying memory.
        /// </summary>
        bool IsPixelContiguous { get; }

        /// <summary>
        /// Gets the underlying memory as a contigous span of pixel bytes.
        /// </summary>
        ReadOnlySpan<byte> GetPixelByteSpan();
    }

    /// <summary>
    /// Represents a read-only view of pixels in contiguous memory.
    /// </summary>
    public interface IReadOnlyPixelMemory<TPixel> : IReadOnlyPixelMemory, IReadOnlyPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the underlying memory as a contigous span of pixels.
        /// </summary>
        ReadOnlySpan<TPixel> GetPixelSpan();
    }
}
