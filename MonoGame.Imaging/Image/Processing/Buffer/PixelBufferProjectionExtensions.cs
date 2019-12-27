using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public delegate IReadOnlyPixelView<TPixel> BufferProjectorCallback<TPixel>(
        ReadOnlyPixelBufferContext<TPixel> context)
        where TPixel : unmanaged, IPixel;

    public static class PixelBufferProjectionExtensions
    {
        public static IReadOnlyPixelView<TPixel> Project<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels,
            BufferProjectorCallback<TPixel> projector, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (projector == null) throw new ArgumentNullException(nameof(projector));
            return projector.Invoke(new ReadOnlyPixelBufferContext<TPixel>(pixels, config));
        }

        public static IReadOnlyPixelView<TPixel> Project<TPixel>(this IReadOnlyPixelBuffer<TPixel> pixels,
            BufferProjectorCallback<TPixel> projector)
            where TPixel : unmanaged, IPixel
        {
            return Project(pixels, projector, ImagingConfig.Default);
        }
    }
}
