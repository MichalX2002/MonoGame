using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    /// <summary>
    /// Context containing a <see cref="IPixelBuffer{TPixel}{T}"/> and <see cref="ImagingConfig"/>.
    /// </summary>
    public readonly struct PixelBufferContext<TPixel>
        where TPixel : unmanaged, IPixel
    {
        public IPixelBuffer<TPixel> Buffer { get; }
        public ImagingConfig Config { get; }

        public PixelBufferContext(IPixelBuffer<TPixel> buffer, ImagingConfig config)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }
    }
}
