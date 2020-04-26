using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IReadOnlyPixelRowsContext : IImagingConfigProvider, IReadOnlyPixelRows
    {
        IReadOnlyPixelRows Pixels { get; }
    }
}