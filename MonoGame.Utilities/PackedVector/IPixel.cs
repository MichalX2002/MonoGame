using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Base interface for pixels, defining the operations to be implemented by a pixel type.
    /// </summary>
    public interface IPixel : IPackedVector
    {
        /// <summary>Sets the pixel value from <see cref="Argb32"/>.</summary>
        void FromArgb32(Argb32 source);

        /// <summary>Sets the pixel value from <see cref="Bgra5551"/>.</summary>
        void FromBgra5551(Bgra5551 source);

        /// <summary>Sets the pixel value from <see cref="Bgr24"/>.</summary>
        void FromBgr24(Bgr24 source);

        /// <summary>Sets the pixel value from <see cref="Bgra32"/>.</summary>
        void FromBgra32(Bgra32 source);

        /// <summary>Sets the pixel value from <see cref="Gray8"/>.</summary>
        void FromGray8(Gray8 source);

        /// <summary>Sets the pixel value from <see cref="Gray16"/>.</summary>
        void FromGray16(Gray16 source);

        /// <summary>Sets the pixel value from <see cref="Rgb24"/>.</summary>
        void FromRgb24(Rgb24 source);

        /// <summary>Sets the pixel value from <see cref="Color"/>.</summary>
        void FromColor(Color source);

        /// <summary>Sets the pixel instance from <see cref="Rgb48"/>.</summary>
        void FromRgb48(Rgb48 source);

        /// <summary>Sets the pixel instance from an <see cref="Rgba64"/>.</summary>
        void FromRgba64(Rgba64 source);

        // /// <summary>
        // /// Converts the pixel instance into <see cref="Gray8"/> representation.
        // /// </summary>
        // /// <param name="dest">The reference to the destination <see cref="Gray8"/> pixel</param>
        // void ToGray8(ref Gray8 dest);
        // 
        // /// <summary>
        // /// Converts the pixel instance into <see cref="GrayAlpha16"/> representation.
        // /// </summary>
        // /// <param name="dest">The reference to the destination <see cref="GrayAlpha16"/> pixel</param>
        // void ToGrayAlpha16(ref GrayAlpha16 dest);
        // 
        // /// <summary>
        // /// Converts the pixel instance into <see cref="Rgb24"/> representation.
        // /// </summary>
        // /// <param name="dest">The reference to the destination <see cref="Rgb24"/> pixel</param>
        // void ToRgb24(ref Rgb24 dest);

        /// <summary>
        /// Converts the pixel value into <see cref="Color"/> representation.
        /// </summary>
        /// <param name="destination">The destination reference for the <see cref="Color"/>.</param>
        void ToColor(ref Color destination);
    }
}
