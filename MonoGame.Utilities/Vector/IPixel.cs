using System;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// The operations to be implemented by a pixel type.
    /// </summary>
    [CLSCompliant(false)]
    public interface IPixel : IVector
    {
        #region From Alpha

        /// <summary>Sets the pixel value from <see cref="Alpha8"/>.</summary>
        void FromAlpha8(Alpha8 source);

        /// <summary>Sets the pixel value from <see cref="Alpha16"/>.</summary>
        void FromAlpha16(Alpha16 source);

        /// <summary>Sets the pixel value from <see cref="AlphaF"/>.</summary>
        void FromAlphaF(AlphaF source);

        #endregion

        #region From Gray

        /// <summary>Sets the pixel value from <see cref="Vector.Gray8"/>.</summary>
        void FromGray8(Gray8 source)
        {
            FromScaledVector3(source.ToScaledVector3());
        }

        /// <summary>Sets the pixel value from <see cref="Vector.Gray16"/>.</summary>
        void FromGray16(Gray16 source)
        {
            FromScaledVector3(source.ToScaledVector3());
        }

        /// <summary>Sets the pixel value from <see cref="Vector.GrayAlpha16"/>.</summary>
        void FromGrayAlpha16(GrayAlpha16 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel value from <see cref="Vector.GrayF"/>.</summary>
        void FromGrayF(GrayF source)
        {
            FromScaledVector3(source.ToScaledVector3());
        }

        #endregion

        #region From Color

        /// <summary>Sets the pixel value from <see cref="Rgb24"/>.</summary>
        void FromRgb24(Rgb24 source)
        {
            FromScaledVector3(source.ToScaledVector3());
        }

        /// <summary>Sets the pixel value from <see cref="Color"/>.</summary>
        void FromRgba32(Color source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel value from <see cref="Rgb48"/>.</summary>
        void FromRgb48(Rgb48 source)
        {
            FromScaledVector3(source.ToScaledVector3());
        }

        /// <summary>Sets the pixel value from an <see cref="Rgba64"/>.</summary>
        void FromRgba64(Rgba64 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        #endregion

        #region From Arbitrary Color

        /// <summary>Sets the pixel value from an <see cref="Bgra5551"/>.</summary>
        void FromBgra5551(Bgra5551 source)
        {
            FromScaledVector4(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel value from an <see cref="Bgr24"/>.</summary>
        void FromBgr24(Bgr24 source)
        {
            FromRgb24(source.ToRgb24());
        }

        /// <summary>Sets the pixel value from an <see cref="Bgra32"/>.</summary>
        void FromBgra32(Bgra32 source)
        {
            FromRgba32(source.ToRgba32());
        }

        /// <summary>Sets the pixel value from an <see cref="Argb32"/>.</summary>
        void FromArgb32(Argb32 source)
        {
            FromRgba32(source.ToRgba32());
        }

        #endregion

        #region To Alpha

        /// <summary>
        /// Gets the alpha of the pixel as an 8-bit integer.
        /// </summary>
        Alpha8 ToAlpha8();

        /// <summary>
        /// Gets the alpha of the pixel as a 16-bit integer.
        /// </summary>
        Alpha16 ToAlpha16();

        /// <summary>
        /// Gets the alpha of the pixel as a 32-bit float.
        /// </summary>
        AlphaF ToAlphaF();

        #endregion

        #region To Gray

        /// <summary>
        /// Gets a gray representation of the pixel as an 8-bit integer.
        /// </summary>
        Gray8 ToGray8() => PackedVectorHelper.Get8BitBT709Luminance(ToRgb24());

        /// <summary>
        /// Gets a gray representation of the pixel as a 16-bit integer.
        /// </summary>
        Gray16 ToGray16() => PackedVectorHelper.Get16BitBT709Luminance(ToRgb48());

        /// <summary>
        /// Gets a gray and alpha representation of the pixel as a pair of 8-bit integers.
        /// </summary>
        GrayAlpha16 ToGrayAlpha16() => PackedVectorHelper.Get8BitBT709LuminanceAlpha(ToRgba32());

        /// <summary>
        /// Gets a gray representation of the pixel as a 32-bit float.
        /// </summary>
        GrayF ToGrayF() => PackedVectorHelper.GetBT709Luminance(ToRgbVector());

        #endregion

        #region To Color

        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 8-bit integer values.
        /// </summary>
        Rgb24 ToRgb24();

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha 8-bit integer values.
        /// </summary>
        Color ToRgba32();

        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 16-bit integer values.
        /// </summary>
        Rgb48 ToRgb48();

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha 16-bit integer values.
        /// </summary>
        Rgba64 ToRgba64();

        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 32-bit float values. 
        /// </summary>
        RgbVector ToRgbVector() => ToScaledVector3();

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha values 32-bit float values.
        /// </summary>
        RgbaVector ToRgbaVector() => ToScaledVector4();

        #endregion
    }

    [CLSCompliant(false)]
    public interface IPixel<TSelf> : IPixel, IVector<TSelf>
        where TSelf : IPixel<TSelf>
    {
    }
}
