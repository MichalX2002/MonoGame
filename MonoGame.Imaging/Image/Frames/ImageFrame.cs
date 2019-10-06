using System;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Represents pixel rows with additional information about it.
    /// </summary>
    /// <typeparam name="TPixel"></typeparam>
    public class ImageFrame<TPixel> : ReadOnlyImageFrame<TPixel>,
        IPixelMemory<TPixel>, IEquatable<ImageFrame<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the pixels assigned to this frame.
        /// </summary>
        public new Image<TPixel> Pixels { get; }

        public int Stride => Pixels.Stride;

        public ImageFrame(Image<TPixel> pixels, int delay) : base(pixels, delay)
        {
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        }

        public void SetPixel(int x, int y, TPixel value) => Pixels.SetPixel(x, y, value);
        public void SetPixelRow(int x, int y, Span<TPixel> row) => Pixels.SetPixelRow(x, y, row);

        public Span<TPixel> GetPixelSpan() => Pixels.GetPixelSpan();
        ReadOnlySpan<TPixel> IReadOnlyPixelMemory<TPixel>.GetPixelSpan() => Pixels.GetPixelSpan();

        public Span<TPixel> GetPixelRowSpan(int row) => Pixels.GetPixelRowSpan(row);
        ReadOnlySpan<TPixel> IReadOnlyPixelBuffer<TPixel>.GetPixelRowSpan(int row) => Pixels.GetPixelRowSpan(row);

        #region Equals + GetHashCode

        public bool Equals(ImageFrame<TPixel> other)
        {
            return Equals(other.Pixels)
                && Delay == other.Delay;
        }

        public override bool Equals(object obj)
        {
            if (obj is ImageFrame<TPixel> frame)
                return Equals(frame);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int code = 7;
                if (Pixels != null)
                    code = code * 31 + Pixels.GetHashCode();
                code = code * 31 + Delay.GetHashCode();
                return code;
            }
        }

        #endregion
    }
}
