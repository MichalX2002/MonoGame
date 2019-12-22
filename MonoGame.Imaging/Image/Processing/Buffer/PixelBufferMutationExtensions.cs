using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public delegate void BufferMutatorCallback<TPixel>(PixelBufferContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelBufferMutationExtensions
    {
        public static void Mutate<TPixel>(this IPixelBuffer<TPixel> pixels,
            BufferMutatorCallback<TPixel> mutator, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (mutator == null) throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelBufferContext<TPixel>(pixels, config));
        }

        public static void Mutate<TPixel>(this IPixelBuffer<TPixel> pixels,
            BufferMutatorCallback<TPixel> mutator)
            where TPixel : unmanaged, IPixel
        {
            Mutate(pixels, mutator, ImagingConfig.Default);
        }
    }
}
