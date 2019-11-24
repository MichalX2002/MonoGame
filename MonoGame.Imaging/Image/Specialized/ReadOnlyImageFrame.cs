using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Represents read-only pixel rows with additional information.
    /// </summary>
    public class ReadOnlyImageFrame<TPixel> : IReadOnlyPixelBuffer<TPixel>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the pixels assigned to this frame.
        /// </summary>
        public IReadOnlyPixelBuffer<TPixel> Pixels { get; }

        public int Width => Pixels.Width;
        public int Height => Pixels.Height;

        /// <summary>
        /// Gets the delay in milliseconds between this and the next frame.
        /// </summary>
        public int Delay { get; }

        public ReadOnlyImageFrame(IReadOnlyPixelBuffer<TPixel> pixels, int delay)
        {
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
            Delay = delay;
        }

        public ReadOnlySpan<TPixel> GetPixelRowSpan(int row) => Pixels.GetPixelRowSpan(row);

        void IDisposable.Dispose()
        {
        }
    }
}
