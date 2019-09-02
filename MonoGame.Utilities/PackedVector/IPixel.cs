using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Base interface for pixels, defining the conversion operations to be implemented by a pixel type.
    /// </summary>
    public interface IPixel : IPackedVector
    {
        /// <summary>
        /// Initializes the pixel instance from an <see cref="Argb32"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Argb32"/> value.</param>
        void FromArgb32(Argb32 source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Bgra5551"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Bgra5551"/> value.</param>
        void FromBgra5551(Bgra5551 source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Bgr24"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Bgr24"/> value.</param>
        void FromBgr24(Bgr24 source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Bgra32"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Bgra32"/> value.</param>
        void FromBgra32(Bgra32 source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Gray8"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Gray8"/> value.</param>
        void FromGray8(Gray8 source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Gray16"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Gray16"/> value.</param>
        void FromGray16(Gray16 source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Rgb24"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Rgb24"/> value.</param>
        void FromRgb24(Rgb24 source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Color"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Color"/> value.</param>
        void FromColor(Color source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Rgb48"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Rgb48"/> value.</param>
        void FromRgb48(Rgb48 source);

        /// <summary>
        /// Initializes the pixel instance from an <see cref="Rgba64"/> value.
        /// </summary>
        /// <param name="source">The <see cref="Rgba64"/> value.</param>
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
        /// Converts the pixel instance into <see cref="Color"/> representation.
        /// </summary>
        /// <param name="dest">The reference to the destination <see cref="Color"/> pixel</param>
        void ToColor(ref Color dest);
    }
}
