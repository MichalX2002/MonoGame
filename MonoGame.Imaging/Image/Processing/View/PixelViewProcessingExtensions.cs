using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public delegate Image<TPixel> BufferProcessorCallback<TPixel>(
        ReadOnlyPixelBufferContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelViewProcessingExtensions
    {
        public static Image<TPixel> Process<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels,
            BufferProcessorCallback<TPixel> processor, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (processor == null) throw new ArgumentNullException(nameof(processor));
            return processor.Invoke(new ReadOnlyPixelBufferContext<TPixel>(pixels, config));
        }

        public static Image<TPixel> Process<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels,
            BufferProcessorCallback<TPixel> processor)
            where TPixel : unmanaged, IPixel
        {
            return Process(pixels, processor, ImagingConfig.Default);
        }
    }
}
