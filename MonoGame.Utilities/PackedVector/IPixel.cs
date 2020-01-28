
namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// The operations to be implemented by a pixel type.
    /// </summary>
    public interface IPixel : IPackedVector
    {
        /// <summary>
        /// Sets the pixel from a scaled <see cref="Vector4"/>.
        /// The XYZW vector components correspond to RGBA. 
        /// </summary>
        void FromScaledVector4(Vector4 vector);

        /// <summary>
        /// Gets the pixel as a scaled <see cref="Vector4"/>.
        /// The XYZW vector components correspond to RGBA. 
        /// </summary>
        Vector4 ToScaledVector4();

        #region FromGray

        /// <summary>Sets the pixel value from <see cref="Gray8"/>.</summary>
        void FromGray8(Gray8 source);

        /// <summary>Sets the pixel value from <see cref="Gray16"/>.</summary>
        void FromGray16(Gray16 source);

        /// <summary>Sets the pixel value from <see cref="GrayAlpha88"/>.</summary>
        void FromGrayAlpha16(GrayAlpha88 source);

        #endregion

        #region FromColor

        /// <summary>Sets the pixel value from <see cref="Rgb24"/>.</summary>
        void FromRgb24(Rgb24 source);

        /// <summary>Sets the pixel value from <see cref="Color"/>.</summary>
        void FromColor(Color source);

        /// <summary>Sets the pixel instance from <see cref="Rgb48"/>.</summary>
        void FromRgb48(Rgb48 source);

        /// <summary>Sets the pixel instance from an <see cref="Rgba64"/>.</summary>
        void FromRgba64(Rgba64 source);

        #endregion

        /// <summary>
        /// Converts the pixel value into <see cref="Color"/>.
        /// </summary>
        /// <param name="destination">The destination reference for the <see cref="Color"/>.</param>
        void ToColor(ref Color destination);
    }
}
