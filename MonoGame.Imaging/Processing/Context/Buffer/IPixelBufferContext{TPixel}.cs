using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IPixelBufferContext<TPixel> : 
        IReadOnlyPixelBufferContext<TPixel>, IPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        new IPixelBuffer<TPixel> Pixels { get; }
    }
}