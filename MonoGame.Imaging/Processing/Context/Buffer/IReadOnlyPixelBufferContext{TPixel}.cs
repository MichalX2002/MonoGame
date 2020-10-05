using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IReadOnlyPixelBufferContext<TPixel> : 
        IReadOnlyPixelBufferContext, IReadOnlyPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        new IReadOnlyPixelBuffer<TPixel> Pixels { get; }
    }
}