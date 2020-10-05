using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate IReadOnlyPixelRows PixelRowsProjectorCallback(ReadOnlyPixelRowsContext context);

    public delegate IReadOnlyPixelRows<TPixel> PixelRowsProjectorCallback<TPixel>(
        ReadOnlyPixelRowsContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelRowsProjectionExtensions
    {
        public static IReadOnlyPixelRows ProjectRows(this IReadOnlyPixelRows pixels,
            ImagingConfig imagingConfig, PixelRowsProjectorCallback projector)
        {
            if (projector == null)
                throw new ArgumentNullException(nameof(projector));
            return projector.Invoke(new ReadOnlyPixelRowsContext(imagingConfig, pixels));
        }

        public static IReadOnlyPixelRows ProjectRows(this IReadOnlyPixelRows pixels,
            PixelRowsProjectorCallback projector)
        {
            return ProjectRows(pixels, ImagingConfig.Default, projector);
        }

        public static IReadOnlyPixelRows<TPixel> ProjectRows<TPixel>(
            this IReadOnlyPixelRows<TPixel> pixels,
            ImagingConfig imagingConfig,
            PixelRowsProjectorCallback<TPixel> projector)
            where TPixel : unmanaged, IPixel
        {
            if (projector == null)
                throw new ArgumentNullException(nameof(projector));
            return projector.Invoke(new ReadOnlyPixelRowsContext<TPixel>(imagingConfig, pixels));
        }

        public static IReadOnlyPixelRows<TPixel> ProjectRows<TPixel>(
            this IReadOnlyPixelRows<TPixel> pixels,
            PixelRowsProjectorCallback<TPixel> projector)
            where TPixel : unmanaged, IPixel
        {
            return ProjectRows(pixels, ImagingConfig.Default, projector);
        }
    }
}
