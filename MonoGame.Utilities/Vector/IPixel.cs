
namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// The operations to be implemented by a pixel type.
    /// </summary>
    public interface IPixel : IVector
    {
        #region FromGray

        /// <summary>Sets the pixel value from <see cref="Gray8"/>.</summary>
        void FromGray8(Gray8 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel value from <see cref="Gray16"/>.</summary>
        void FromGray16(Gray16 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel value from <see cref="GrayAlpha16"/>.</summary>
        void FromGrayAlpha16(GrayAlpha16 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        #endregion

        #region FromColor

        /// <summary>Sets the pixel value from <see cref="Rgb24"/>.</summary>
        void FromRgb24(Rgb24 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel value from <see cref="Color"/>.</summary>
        void FromColor(Color source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel instance from <see cref="Rgb48"/>.</summary>
        void FromRgb48(Rgb48 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel instance from an <see cref="Rgba64"/>.</summary>
        void FromRgba64(Rgba64 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        #endregion

        /// <summary>
        /// Converts the pixel value into <see cref="Color"/>.
        /// </summary>
        /// <param name="destination">The destination reference for the <see cref="Color"/>.</param>
        Color ToColor()
        {
            var color = new Color(); // TODO: Unsafe.SkipInit
            color.FromScaledVector4(ToScaledVector4());

            return color;
        }
    }

    public interface IPixel<TSelf> : IPixel, IVector<TSelf>
        where TSelf : IPixel<TSelf>
    {
    }
}
