using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IPixelBufferContext : IReadOnlyPixelBufferContext, IPixelBuffer
    {
        new IPixelBuffer Pixels { get; }
    }
}