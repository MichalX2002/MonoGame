using MonoGame.Framework;

namespace MonoGame.Imaging.Processing
{
    /// <summary>
    /// Represents a progress update for image processing.
    /// </summary>
    public delegate void ProcessingProgressCallback<TState>(
        TState state,
        float percentage,
        Rectangle? rectangle);

    public static partial class PixelRowsProcessor
    {
    }
}
