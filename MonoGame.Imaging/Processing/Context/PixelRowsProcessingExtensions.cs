using System;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate Image RowsProcessorCallback(ReadOnlyPixelRowsContext context);
    public delegate Image<TPixel> RowsProcessorCallback<TPixel>(ReadOnlyPixelRowsContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelRowsProcessingExtensions
    {
        public static Image Process(this IReadOnlyPixelRows pixels, 
            ImagingConfig config, RowsProcessorCallback processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            return processor.Invoke(new ReadOnlyPixelRowsContext(config, pixels));
        }

        public static Image Process(this IReadOnlyPixelRows pixels, 
            RowsProcessorCallback processor)
        {
            return Process(pixels, ImagingConfig.Default, processor);
        }

        public static Image<TPixel> Process<TPixel>(this IReadOnlyPixelRows<TPixel> pixels, 
            ImagingConfig config, RowsProcessorCallback<TPixel> processor)
            where TPixel : unmanaged, IPixel
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            return processor.Invoke(new ReadOnlyPixelRowsContext<TPixel>(config, pixels));
        }

        public static Image<TPixel> Process<TPixel>(this IReadOnlyPixelRows<TPixel> pixels, 
            RowsProcessorCallback<TPixel> processor)
            where TPixel : unmanaged, IPixel
        {
            return Process(pixels, ImagingConfig.Default, processor);
        }
    }
}
