using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a read-only view of pixels.
    /// </summary>
    /// <typeparam name="TPixel"></typeparam>
    public interface IReadOnlyPixelView<TPixel> : IPixelSource<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets a single pixel at the given coordinates.
        /// </summary>
        /// <param name="x">The offset in the row.</param>
        /// <param name="y">The row.</param>
        /// <returns>The pixel.</returns>
        TPixel GetPixel(int x, int y);
    }
}
