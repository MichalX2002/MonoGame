using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IPixelRowsContext : IReadOnlyPixelRowsContext, IPixelRows
    {
        new IPixelRows Pixels { get; }
    }
}