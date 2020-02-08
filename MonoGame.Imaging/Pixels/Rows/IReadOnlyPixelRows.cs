using System;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only view of pixel rows.
    /// </summary>
    public interface IReadOnlyPixelRows<TPixel> : IReadOnlyPixelView<TPixel>
        where TPixel : unmanaged, IPixel
    {
        void GetPixelRow(int x, int y, Span<TPixel> destination);
    }
}