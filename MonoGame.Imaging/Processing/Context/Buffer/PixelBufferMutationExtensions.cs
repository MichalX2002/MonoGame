using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate void PixelBufferMutatorCallback(IPixelBufferContext context);

    public delegate void PixelBufferMutatorCallback<TPixel>(IPixelBufferContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelBufferMutationExtensions
    {
        public static void MutateBuffer(this IPixelBuffer pixels,
            ImagingConfig imagingConfig, PixelBufferMutatorCallback mutator)
        {
            if (mutator == null) 
                throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelBufferContext(imagingConfig, pixels));
        }

        public static void MutateBuffer(this IPixelBuffer pixels,
            PixelBufferMutatorCallback mutator)
        {
            MutateBuffer(pixels, ImagingConfig.Default, mutator);
        }

        public static void MutateBuffer<TPixel>(this IPixelBuffer<TPixel> pixels, 
            ImagingConfig imagingConfig, PixelBufferMutatorCallback<TPixel> mutator)
            where TPixel : unmanaged, IPixel
        {
            if (mutator == null)
                throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelBufferContext<TPixel>(imagingConfig, pixels));
        }

        public static void MutateBuffer<TPixel>(this IPixelBuffer<TPixel> pixels, 
            PixelBufferMutatorCallback<TPixel> mutator)
            where TPixel : unmanaged, IPixel
        {
            MutateBuffer(pixels, ImagingConfig.Default, mutator);
        }
    }
}
