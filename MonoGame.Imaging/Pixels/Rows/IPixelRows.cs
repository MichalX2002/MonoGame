using System;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a view of pixel rows.
    /// </summary>
    public interface IPixelRows<TPixel> : IReadOnlyPixelRows<TPixel>, IPixelView<TPixel>
        where TPixel : unmanaged, IPixel
    {
        void SetPixelRow(int x, int y, Span<TPixel> row);
    }
}
