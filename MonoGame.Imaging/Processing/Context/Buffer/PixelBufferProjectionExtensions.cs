using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate IReadOnlyPixelBuffer PixelBufferProjectorCallback(ReadOnlyPixelBufferContext context);

    public delegate IReadOnlyPixelBuffer<TPixel> PixelBufferProjectorCallback<TPixel>(
        ReadOnlyPixelBufferContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelBufferProjectionExtensions
    {
        public static IReadOnlyPixelBuffer ProjectRows(this IReadOnlyPixelBuffer pixels,
            ImagingConfig imagingConfig, PixelBufferProjectorCallback projector)
        {
            if (projector == null)
                throw new ArgumentNullException(nameof(projector));
            return projector.Invoke(new ReadOnlyPixelBufferContext(imagingConfig, pixels));
        }

        public static IReadOnlyPixelBuffer ProjectRows(this IReadOnlyPixelBuffer pixels,
            PixelBufferProjectorCallback projector)
        {
            return ProjectRows(pixels, ImagingConfig.Default, projector);
        }

        public static IReadOnlyPixelBuffer<TPixel> ProjectRows<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels,
            ImagingConfig imagingConfig,
            PixelBufferProjectorCallback<TPixel> projector)
            where TPixel : unmanaged, IPixel
        {
            if (projector == null)
                throw new ArgumentNullException(nameof(projector));
            return projector.Invoke(new ReadOnlyPixelBufferContext<TPixel>(imagingConfig, pixels));
        }

        public static IReadOnlyPixelBuffer<TPixel> ProjectRows<TPixel>(
            this IReadOnlyPixelBuffer<TPixel> pixels, 
            PixelBufferProjectorCallback<TPixel> projector)
            where TPixel : unmanaged, IPixel
        {
            return ProjectRows(pixels, ImagingConfig.Default, projector);
        }
    }
}
