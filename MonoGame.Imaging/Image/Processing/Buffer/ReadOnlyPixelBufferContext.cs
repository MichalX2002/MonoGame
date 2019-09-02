using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    /// <summary>
    /// Context containing a <see cref="IReadOnlyPixelBuffer{TPixel}{T}"/> and <see cref="ImagingConfig"/>.
    /// </summary>
    public readonly struct ReadOnlyPixelBufferContext<TPixel>
        where TPixel : unmanaged, IPixel
    {
        public IReadOnlyPixelBuffer<TPixel> Buffer { get; }
        public ImagingConfig Config { get; }

        public ReadOnlyPixelBufferContext(IReadOnlyPixelBuffer<TPixel> buffer, ImagingConfig config)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }
    }
}
