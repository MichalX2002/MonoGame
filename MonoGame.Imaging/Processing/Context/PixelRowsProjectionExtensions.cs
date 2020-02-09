using System;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate IReadOnlyPixelRows ViewProjectorCallback(ReadOnlyPixelRowsContext context);
    public delegate IReadOnlyPixelRows<TPixel> ViewProjectorCallback<TPixel>(
        ReadOnlyPixelRowsContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

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

        public static IReadOnlyPixelRows<TPixel> Project<TPixel>(this IReadOnlyPixelRows<TPixel> pixels,
            ImagingConfig config, ViewProjectorCallback<TPixel> projector)
            where TPixel : unmanaged, IPixel
        {
            if (projector == null)
                throw new ArgumentNullException(nameof(projector));
            return projector.Invoke(new ReadOnlyPixelRowsContext<TPixel>(config, pixels));
        }

        public static IReadOnlyPixelRows<TPixel> Project<TPixel>(this IReadOnlyPixelRows<TPixel> pixels,
            ViewProjectorCallback<TPixel> projector)
            where TPixel : unmanaged, IPixel
        {
            return Project(pixels, ImagingConfig.Default, projector);
        }
    }
}
