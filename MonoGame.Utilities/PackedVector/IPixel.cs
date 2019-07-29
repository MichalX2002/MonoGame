
namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
    /// <summary>
    /// Base interface for pixels, defining the conversion operations to be implemented by a pixel type.
    /// </summary>
    public interface IPixel : IPackedVector
    {
        /// <summary>
        /// Initializes the pixel instance from a generic ("scaled") <see cref="Vector4"/>.
        /// </summary>
        /// <param name="vector">The vector to load the pixel from.</param>
        void FromScaledVector4(Vector4 vector);

        /// <summary>
        /// Expands the pixel into a generic ("scaled") <see cref="Vector4"/> representation
        /// with values scaled and clamped between <value>0</value> and <value>1</value>.
        /// The vector components are typically expanded in least to greatest significance order.
        /// </summary>
        /// <returns>The <see cref="Vector4"/>.</returns>
        Vector4 ToScaledVector4();

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

        /// <summary>
        /// Convert the pixel instance into <see cref="Color"/> representation.
        /// </summary>
        /// <param name="dest">The reference to the destination <see cref="Color"/> pixel</param>
        void ToColor(ref Color dest);
    }
}
