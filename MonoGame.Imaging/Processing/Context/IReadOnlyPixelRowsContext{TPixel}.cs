using MonoGame.Framework.Vector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IReadOnlyPixelRowsContext<TPixel> : IReadOnlyPixelRowsContext, IReadOnlyPixelRows<TPixel>
        where TPixel : unmanaged, IPixel
    {
        new IReadOnlyPixelRows<TPixel> Pixels { get; }
    }
}