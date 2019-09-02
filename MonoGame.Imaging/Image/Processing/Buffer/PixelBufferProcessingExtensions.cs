using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public delegate Image<TPixel> BufferProcessorCallback<TPixel>(
        ReadOnlyPixelBufferContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelBufferProcessingExtensions
    {
        public static Image<TPixel> Process<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels,
            BufferProcessorCallback<TPixel> mutation, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (mutation == null) throw new ArgumentNullException(nameof(mutation));
            return mutation.Invoke(new ReadOnlyPixelBufferContext<TPixel>(pixels, config));
        }

        public static Image<TPixel> Process<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels,
            BufferProcessorCallback<TPixel> mutation)
            where TPixel : unmanaged, IPixel
        {
            return Process(pixels, mutation, ImagingConfig.Default);
        }
    }
}
