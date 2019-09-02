using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    /// <summary>
    /// Context containing a <see cref="IReadOnlyPixelView{TPixel}{T}"/> and <see cref="ImagingConfig"/>.
    /// </summary>
    public readonly struct ReadOnlyPixelViewContext<TPixel>
        where TPixel : unmanaged, IPixel
    {
        public IReadOnlyPixelView<TPixel> View { get; }
        public ImagingConfig Config { get; }

        public ReadOnlyPixelViewContext(IReadOnlyPixelView<TPixel> view, ImagingConfig config)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }
    }
}
