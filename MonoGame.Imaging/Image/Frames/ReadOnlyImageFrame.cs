using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Represents read-only pixel rows with additional information.
    /// </summary>
    public class ReadOnlyImageFrame<TPixel> :
        IReadOnlyPixelBuffer<TPixel>, IEquatable<ReadOnlyImageFrame<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the delay in milliseconds between this and the next frame.
        /// </summary>
        public int Delay { get; }

        /// <summary>
        /// Gets the pixels assigned to this frame.
        /// </summary>
        public IReadOnlyPixelBuffer<TPixel> Pixels { get; }

        public int Width => Pixels.Width;
        public int Height => Pixels.Height;

        public ReadOnlyImageFrame(IReadOnlyPixelBuffer<TPixel> pixels, int delay)
        {
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
            Delay = delay;
        }

        public ReadOnlySpan<TPixel> GetPixelRowSpan(int row) => Pixels.GetPixelRowSpan(row);

        #region Object Overrides

        public bool Equals(ReadOnlyImageFrame<TPixel> other)
        {
            return Equals(other.Pixels)
                && Delay == other.Delay;
        }

        public override bool Equals(object obj)
        {
            return obj is ReadOnlyImageFrame<TPixel> frame && Equals(frame);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int code = 7;
                if (Pixels != null)
                    code = code * 21 + Pixels.GetHashCode();
                code = code * 21 + Delay.GetHashCode();
                return code;
            }
        }

        #endregion

        void IDisposable.Dispose()
        {
        }
    }
}
