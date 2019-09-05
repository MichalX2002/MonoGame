using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Represents read-only pixel rows with additional information about it.
    /// </summary>
    /// <typeparam name="TPixel"></typeparam>
    public class ReadOnlyImageFrame<TPixel> :
        IReadOnlyPixelRows<TPixel>, IEquatable<ReadOnlyImageFrame<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the pixels assigned to this frame.
        /// </summary>
        public IReadOnlyPixelRows<TPixel> Pixels { get; }

        /// <summary>
        /// Gets the delay in milliseconds between this and the next frame.
        /// </summary>
        public int Delay { get; }

        public int Width => Pixels.Width;
        public int Height => Pixels.Height;

        public ReadOnlyImageFrame(IReadOnlyPixelRows<TPixel> pixels, int delay)
        {
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
            Delay = delay;
        }

        public TPixel GetPixel(int x, int y) => Pixels.GetPixel(x, y);
        public void GetPixelRow(int x, int y, Span<TPixel> destination) => Pixels.GetPixelRow(x, y, destination);

        #region Equals + GetHashCode

        public bool Equals(ReadOnlyImageFrame<TPixel> other)
        {
            return Equals(other.Pixels)
                && Delay == other.Delay;
        }

        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyImageFrame<TPixel> frame)
                return Equals(frame);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int code = 17;
                if (Pixels != null)
                    code = (code * 23) + Pixels.GetHashCode();
                code = (code * 23) + Delay.GetHashCode();
                return code;
            }
        }

        #endregion

        void IDisposable.Dispose()
        {
        }
    }
}
