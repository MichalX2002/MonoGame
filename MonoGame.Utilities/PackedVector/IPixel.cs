
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
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        /// <summary>Sets the pixel value from <see cref="Gray16"/>.</summary>
        void FromGray16(Gray16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        /// <summary>Sets the pixel value from <see cref="GrayAlpha16"/>.</summary>
        void FromGrayAlpha16(GrayAlpha16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        #endregion

        #region FromColor

        /// <summary>Sets the pixel value from <see cref="Rgb24"/>.</summary>
        void FromRgb24(Rgb24 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        /// <summary>Sets the pixel value from <see cref="Color"/>.</summary>
        void FromColor(Color source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        /// <summary>Sets the pixel instance from <see cref="Rgb48"/>.</summary>
        void FromRgb48(Rgb48 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        /// <summary>Sets the pixel instance from an <see cref="Rgba64"/>.</summary>
        void FromRgba64(Rgba64 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        #endregion

        /// <summary>
        /// Converts the pixel value into <see cref="Color"/>.
        /// </summary>
        /// <param name="destination">The destination reference for the <see cref="Color"/>.</param>
        void ToColor(out Color destination)
        {
            destination = default; // TODO: Unsafe.SkipInit

            ToVector4(out var vector);
            destination.FromVector4(vector);
        }
    }

    public interface IPixel<TSelf> : IPixel, IVector<TSelf>
        where TSelf : IPixel<TSelf>
    {
    }
}
