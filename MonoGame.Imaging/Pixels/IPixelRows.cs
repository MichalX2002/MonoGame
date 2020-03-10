using System;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only way to get pixel rows in bytes.
    /// </summary>
    public interface IPixelRows : IReadOnlyPixelRows
    {
        /// <summary>
        /// Sets a row of pixel bytes.
        /// </summary>
        /// <param name="x">The column in the row to start setting in pixels.</param>
        /// <param name="y">The row to set in pixels.</param>
        void SetPixelByteRow(int x, int y, ReadOnlySpan<byte> data);
    }

    /// <summary>
    /// Represents a read-only way to get pixel rows.
    /// </summary>
    public interface IPixelRows<TPixel> : IPixelRows, IReadOnlyPixelRows<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Sets a row of pixels.
        /// </summary>
        /// <param name="x">The column in the row to start setting in pixels.</param>
        /// <param name="y">The row to set in pixels.</param>
        void SetPixelRow(int x, int y, ReadOnlySpan<TPixel> data);
    }
}
