using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public interface IReadOnlyPixelRowsContext : IImagingConfigurable, IReadOnlyPixelRows
    {
        IReadOnlyPixelRows Pixels { get; }
    }
}