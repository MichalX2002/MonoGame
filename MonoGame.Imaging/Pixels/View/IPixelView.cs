using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Represents a view of pixels.
    /// </summary>
    public interface IPixelView<TPixel> : IReadOnlyPixelView<TPixel>
        where TPixel : unmanaged, IPixel
    {
        void SetPixel(int x, int y, TPixel value);
    }
}
