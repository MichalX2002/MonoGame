using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate IReadOnlyPixelRows ViewProjectorCallback(ReadOnlyPixelRowsContext context);

    public static class PixelRowsProjectionExtensions
    {
        public static IReadOnlyPixelRows Project(this IReadOnlyPixelRows pixels,
            ImagingConfig config, ViewProjectorCallback projector)
        {
            if (projector == null)
                throw new ArgumentNullException(nameof(projector));
            return projector.Invoke(new ReadOnlyPixelRowsContext(config, pixels));
        }

        public static IReadOnlyPixelRows Project(this IReadOnlyPixelRows pixels,
            ViewProjectorCallback projector)
        {
            return Project(pixels, ImagingConfig.Default, projector);
        }
    }
}
