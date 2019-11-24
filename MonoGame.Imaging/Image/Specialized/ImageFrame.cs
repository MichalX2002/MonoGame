using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Represents pixel rows with additional information about it.
    /// </summary>
    public class ImageFrame<TPixel> : ReadOnlyImageFrame<TPixel>, IPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the pixels assigned to this frame.
        /// </summary>
        public new IPixelBuffer<TPixel> Pixels { get; }

        public ImageFrame(IPixelBuffer<TPixel> pixels, int delay) : base(pixels, delay)
        {
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        }

        public new Span<TPixel> GetPixelRowSpan(int row) => Pixels.GetPixelRowSpan(row);
    }
}
