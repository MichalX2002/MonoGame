using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IReadOnlyPixelBufferContext : IReadOnlyPixelRowsContext, IReadOnlyPixelBuffer
    {
        new IReadOnlyPixelBuffer Pixels { get; }
    }
}