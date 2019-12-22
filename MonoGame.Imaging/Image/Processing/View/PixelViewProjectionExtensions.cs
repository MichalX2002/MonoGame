using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public delegate IReadOnlyPixelView<TPixel> ViewProjectorCallback<TPixel>(
        ReadOnlyPixelViewContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelViewProjectionExtensions
    {
        public static IReadOnlyPixelView<TPixel> Project<TPixel>(this IReadOnlyPixelView<TPixel> pixels,
            ViewProjectorCallback<TPixel> projector, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (projector == null) throw new ArgumentNullException(nameof(projector));
            return projector.Invoke(new ReadOnlyPixelViewContext<TPixel>(pixels, config));
        }

        public static IReadOnlyPixelView<TPixel> Project<TPixel>(this IReadOnlyPixelView<TPixel> pixels,
            ViewProjectorCallback<TPixel> projector)
            where TPixel : unmanaged, IPixel
        {
            return Project(pixels, projector, ImagingConfig.Default);
        }
    }
}
