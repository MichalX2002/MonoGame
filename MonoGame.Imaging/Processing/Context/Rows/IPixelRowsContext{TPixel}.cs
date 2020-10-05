using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IPixelRowsContext<TPixel> : IReadOnlyPixelRowsContext<TPixel>, IPixelRows<TPixel>
        where TPixel : unmanaged, IPixel
    {
        new IPixelRows<TPixel> Pixels { get; }
    }
}