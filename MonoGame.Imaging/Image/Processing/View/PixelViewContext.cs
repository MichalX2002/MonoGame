using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    /// <summary>
    /// Context containing a <see cref="IPixelView{TPixel}{T}"/> and <see cref="ImagingConfig"/>.
    /// </summary>
    public readonly struct PixelViewContext<TPixel>
        where TPixel : unmanaged, IPixel
    {
        public IPixelView<TPixel> View { get; }
        public ImagingConfig Config { get; }

        public PixelViewContext(IPixelView<TPixel> view, ImagingConfig config)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }
    }
}