using System;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only way to get pixel rows in bytes.
    /// </summary>
    public interface IReadOnlyPixelRows : IElementContainer, IPixelSource
    {
        /// <summary>
        /// Gets a row of pixel bytes.
        /// </summary>
        /// <param name="x">The column in the row to get in pixels.</param>
        /// <param name="y">The row to get in pixels.</param>
        /// <param name="destination">The destination to store the pixel bytes.</param>
        void GetPixelByteRow(int x, int y, Span<byte> destination);
    }

    /// <summary>
    /// Represents a read-only way to get pixel rows.
    /// </summary>
    public interface IReadOnlyPixelRows<TPixel> : IReadOnlyPixelRows
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets a row of pixels.
        /// </summary>
        /// <param name="x">The column in the row to get in pixels.</param>
        /// <param name="y">The row to get in pixels.</param>
        /// <param name="destination">The destination to store the pixels.</param>
        void GetPixelRow(int x, int y, Span<TPixel> destination);
    }
}
