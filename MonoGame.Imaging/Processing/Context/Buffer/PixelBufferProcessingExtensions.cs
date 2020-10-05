using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate Image PixelBufferProcessorCallback(ReadOnlyPixelBufferContext context);

    public delegate Image<TPixel> PixelBufferProcessorCallback<TPixel>(
        ReadOnlyPixelBufferContext<TPixel> context)
        where TPixel : unmanaged, IPixel<TPixel>;

    public static class PixelBufferProcessingExtensions
    {
        public static Image ProcessBuffer(this IReadOnlyPixelBuffer pixels, 
            ImagingConfig imagingConfig, PixelBufferProcessorCallback processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            return processor.Invoke(new ReadOnlyPixelBufferContext(imagingConfig, pixels));
        }

        public static Image ProcessBuffer(this IReadOnlyPixelBuffer pixels, 
            PixelBufferProcessorCallback processor)
        {
            return ProcessBuffer(pixels, ImagingConfig.Default, processor);
        }

        public static Image<TPixel> ProcessBuffer<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels, 
            ImagingConfig imagingConfig, PixelBufferProcessorCallback<TPixel> processor)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            return processor.Invoke(new ReadOnlyPixelBufferContext<TPixel>(imagingConfig, pixels));
        }

        public static Image<TPixel> ProcessBuffer<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels, 
            PixelBufferProcessorCallback<TPixel> processor)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return ProcessBuffer(pixels, ImagingConfig.Default, processor);
        }
    }
}
