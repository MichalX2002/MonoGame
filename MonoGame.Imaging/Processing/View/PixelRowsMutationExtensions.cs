using System;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public delegate void ViewMutatorCallback(PixelRowsContext context);

    public static class PixelRowsMutationExtensions
    {
        public static void Mutate(this IPixelRows pixels,
            ImagingConfig config, ViewMutatorCallback mutator)
        {
            if (mutator == null) 
                throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelRowsContext(config, pixels));
        }

        public static void Mutate<TPixel>(this IPixelRows pixels,
            ViewMutatorCallback mutator)
        {
            Mutate(pixels, ImagingConfig.Default, mutator);
        }
    }
}
