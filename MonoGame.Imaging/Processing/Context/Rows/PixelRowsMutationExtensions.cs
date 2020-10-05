using System;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate void PixelRowsMutatorCallback(IPixelRowsContext context);

    public delegate void PixelRowsMutatorCallback<TPixel>(IPixelRowsContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelRowsMutationExtensions
    {
        public static void MutateRows(this IPixelRows pixels,
            ImagingConfig imagingConfig, PixelRowsMutatorCallback mutator)
        {
            if (mutator == null) 
                throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelRowsContext(imagingConfig, pixels));
        }

        public static void MutateRows(this IPixelRows pixels,
            PixelRowsMutatorCallback mutator)
        {
            MutateRows(pixels, ImagingConfig.Default, mutator);
        }

        public static void MutateRows<TPixel>(
            this IPixelRows<TPixel> pixels, ImagingConfig imagingConfig, PixelRowsMutatorCallback<TPixel> mutator)
            where TPixel : unmanaged, IPixel
        {
            if (mutator == null)
                throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelRowsContext<TPixel>(imagingConfig, pixels));
        }

        public static void MutateRows<TPixel>(
            this IPixelRows<TPixel> pixels, PixelRowsMutatorCallback<TPixel> mutator)
            where TPixel : unmanaged, IPixel
        {
            MutateRows(pixels, ImagingConfig.Default, mutator);
        }
    }
}
