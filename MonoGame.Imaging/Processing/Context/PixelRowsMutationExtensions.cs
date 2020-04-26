using System;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate void ViewMutatorCallback(IPixelRowsContext context);

    public delegate void ViewMutatorCallback<TPixel>(IPixelRowsContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelRowsMutationExtensions
    {
        public static void Mutate(this IPixelRows pixels,
            ImagingConfig imagingConfig, ViewMutatorCallback mutator)
        {
            if (mutator == null) 
                throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelRowsContext(imagingConfig, pixels));
        }

        public static void Mutate(this IPixelRows pixels,
            ViewMutatorCallback mutator)
        {
            Mutate(pixels, ImagingConfig.Default, mutator);
        }

        public static void Mutate<TPixel>(
            this IPixelRows<TPixel> pixels, ImagingConfig imagingConfig, ViewMutatorCallback<TPixel> mutator)
            where TPixel : unmanaged, IPixel
        {
            if (mutator == null)
                throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelRowsContext<TPixel>(imagingConfig, pixels));
        }

        public static void Mutate<TPixel>(
            this IPixelRows<TPixel> pixels, ViewMutatorCallback<TPixel> mutator)
            where TPixel : unmanaged, IPixel
        {
            Mutate(pixels, ImagingConfig.Default, mutator);
        }
    }
}
