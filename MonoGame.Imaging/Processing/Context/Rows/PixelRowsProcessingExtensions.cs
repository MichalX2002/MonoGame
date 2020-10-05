using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate Image PixelRowsProcessorCallback(ReadOnlyPixelRowsContext context);

    public delegate Image<TPixel> PixelRowsProcessorCallback<TPixel>(
        ReadOnlyPixelRowsContext<TPixel> context)
        where TPixel : unmanaged, IPixel<TPixel>;

    public static class PixelRowsProcessingExtensions
    {
        public static Image ProcessRows(
            this IReadOnlyPixelRows pixels, 
            ImagingConfig imagingConfig, PixelRowsProcessorCallback processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            return processor.Invoke(new ReadOnlyPixelRowsContext(imagingConfig, pixels));
        }

        public static Image ProcessRows(
            this IReadOnlyPixelRows pixels, 
            PixelRowsProcessorCallback processor)
        {
            return ProcessRows(pixels, ImagingConfig.Default, processor);
        }

        public static Image<TPixel> ProcessRows<TPixel>(
            this IReadOnlyPixelRows<TPixel> pixels, 
            ImagingConfig imagingConfig, PixelRowsProcessorCallback<TPixel> processor)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            return processor.Invoke(new ReadOnlyPixelRowsContext<TPixel>(imagingConfig, pixels));
        }

        public static Image<TPixel> ProcessRows<TPixel>(
            this IReadOnlyPixelRows<TPixel> pixels, 
            PixelRowsProcessorCallback<TPixel> processor)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return ProcessRows(pixels, ImagingConfig.Default, processor);
        }
    }
}
