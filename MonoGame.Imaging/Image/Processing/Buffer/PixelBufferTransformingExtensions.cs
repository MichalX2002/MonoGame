using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public delegate Image<TPixel> BufferTransformerCallback<TPixel>(
        ReadOnlyPixelBufferContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelBufferProcessingExtensions
    {
        public static Image<TPixel> Transform<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels,
            BufferTransformerCallback<TPixel> mutation, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (mutation == null) throw new ArgumentNullException(nameof(mutation));
            return mutation.Invoke(new ReadOnlyPixelBufferContext<TPixel>(pixels, config));
        }

        public static Image<TPixel> Transform<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels,
            BufferTransformerCallback<TPixel> mutation)
            where TPixel : unmanaged, IPixel
        {
            return Transform(pixels, mutation, ImagingConfig.Default);
        }
    }
}
