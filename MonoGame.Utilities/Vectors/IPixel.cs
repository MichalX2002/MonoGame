
using System;
using System.Numerics;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// The operations to be implemented by a pixel type.
    /// </summary>
    public interface IPixel : IVector
    {
        #region From Alpha

        /// <summary>Sets the pixel value from <see cref="Alpha8"/>.</summary>
        void FromAlpha(Alpha8 source);

        /// <summary>Sets the pixel value from <see cref="Alpha16"/>.</summary>
        void FromAlpha(Alpha16 source);

        /// <summary>Sets the pixel value from <see cref="Alpha32"/>.</summary>
        void FromAlpha(Alpha32 source);

        /// <summary>Sets the pixel value from <see cref="AlphaF"/>.</summary>
        void FromAlpha(AlphaF source);

        #endregion

        #region From Gray

        /// <summary>Sets the pixel value from <see cref="Gray8"/>.</summary>
        void FromGray(Gray8 source) => FromColor(source.ToRgb24());

        /// <summary>Sets the pixel value from <see cref="GrayAlpha16"/>.</summary>
        void FromGray(GrayAlpha16 source) => FromColor(source.ToRgba32());

        /// <summary>Sets the pixel value from <see cref="Gray16"/>.</summary>
        void FromGray(Gray16 source) => FromColor(source.ToRgb48());

        /// <summary>Sets the pixel value from <see cref="Gray32"/>.</summary>
        void FromGray(Gray32 source) => FromScaledVector(source.ToScaledVector3());

        /// <summary>Sets the pixel value from <see cref="GrayF"/>.</summary>
        void FromGray(GrayF source) => FromScaledVector(source.ToScaledVector3());

        #endregion

        #region From Color

        /// <summary>Sets the pixel value from <see cref="Rgb24"/>.</summary>
        void FromColor(Rgb24 source) => FromScaledVector(source.ToScaledVector3());

        /// <summary>Sets the pixel value from <see cref="Rgb48"/>.</summary>
        void FromColor(Rgb48 source) => FromScaledVector(source.ToScaledVector3());

        /// <summary>Sets the pixel value from <see cref="Color"/>.</summary>
        void FromColor(Color source) => FromScaledVector(source.ToScaledVector4());

        /// <summary>Sets the pixel value from <see cref="Rgba1010102"/>.</summary>
        void FromColor(Rgba1010102 source) => FromScaledVector(source.ToScaledVector4());

        /// <summary>Sets the pixel value from an <see cref="Rgba64"/>.</summary>
        void FromColor(Rgba64 source) => FromScaledVector(source.ToScaledVector4());

        #endregion

        #region From Arbitrary Color

        /// <summary>Sets the pixel value from an <see cref="Bgr565"/>.</summary>
        void FromColor(Bgr565 source) => FromColor(source.ToRgb24());

        /// <summary>Sets the pixel value from an <see cref="Bgr24"/>.</summary>
        void FromColor(Bgr24 source) => FromColor(source.ToRgb24());

        /// <summary>Sets the pixel value from an <see cref="Bgra4444"/>.</summary>
        void FromColor(Bgra4444 source) => FromColor(source.ToRgba32());

        /// <summary>Sets the pixel value from an <see cref="Bgra5551"/>.</summary>
        void FromColor(Bgra5551 source) => FromColor(source.ToRgba32());

        /// <summary>Sets the pixel value from an <see cref="Bgra32"/>.</summary>
        void FromColor(Bgra32 source) => FromColor(source.ToRgba32());

        /// <summary>Sets the pixel value from an <see cref="Abgr32"/>.</summary>
        void FromColor(Abgr32 source) => FromColor(source.ToRgba32());

        /// <summary>Sets the pixel value from an <see cref="Argb32"/>.</summary>
        void FromColor(Argb32 source) => FromColor(source.ToRgba32());

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
        Gray8 ToGray8(); // => LuminanceHelper.BT709.ToGray8(ToRgb24());

        /// <summary>
        /// Gets a gray representation of the pixel as a 16-bit integer.
        /// </summary>
        Gray16 ToGray16(); // => LuminanceHelper.BT709.ToGray16(ToRgb48());

        /// <summary>
        /// Gets a gray and alpha representation of the pixel as a pair of 8-bit integers.
        /// </summary>
        GrayAlpha16 ToGrayAlpha16(); // => LuminanceHelper.BT709.ToGrayAlpha16(ToRgba32());

        /// <summary>
        /// Gets a gray representation of the pixel as a 32-bit float.
        /// </summary>
        GrayF ToGrayF(); // => LuminanceHelper.BT709.ToGrayF(ToRgbVector());

        #endregion

        #region To Color

        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 8-bit integer values.
        /// </summary>
        /// <remarks>
        /// <see cref="ScaledVectorHelper.ToRgb24"/> is the default scaled vector implementation.
        /// </remarks>
        Rgb24 ToRgb24();

        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 16-bit integer values.
        /// </summary>
        /// <remarks>
        /// <see cref="ScaledVectorHelper.ToRgb48"/> is the default scaled vector implementation.
        /// </remarks>
        Rgb48 ToRgb48();

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha 8-bit integer values.
        /// </summary>
        /// <remarks>
        /// <see cref="ScaledVectorHelper.ToRgba32"/> is the default scaled vector implementation.
        /// </remarks>
        Color ToRgba32();

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha 16-bit integer values.
        /// </summary>
        /// <remarks>
        /// <see cref="ScaledVectorHelper.ToRgba64"/> is the default scaled vector implementation.
        /// </remarks>
        Rgba64 ToRgba64();

        #endregion
    }

    public interface IPixel<TSelf> : IPixel, IEquatable<TSelf>
        where TSelf : IPixel<TSelf>
    {
    }
}
