using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public delegate void ViewMutatorCallback<TPixel>(PixelViewContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelViewMutationExtensions
    {
        public static void Mutate<TPixel>(this IPixelView<TPixel> pixels,
            ViewMutatorCallback<TPixel> mutator, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (mutator == null) throw new ArgumentNullException(nameof(mutator));
            mutator.Invoke(new PixelViewContext<TPixel>(pixels, config));
        }

        public static void Mutate<TPixel>(this IPixelView<TPixel> pixels,
            ViewMutatorCallback<TPixel> mutator)
            where TPixel : unmanaged, IPixel
        {
            Mutate(pixels, mutator, ImagingConfig.Default);
        }
    }
}
