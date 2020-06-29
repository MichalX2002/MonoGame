
namespace MonoGame.Framework.Vector
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

        /// <summary>Sets the pixel value from <see cref="AlphaF"/>.</summary>
        void FromAlpha(AlphaF source);

        #endregion

        #region From Gray

        /// <summary>Sets the pixel value from <see cref="Gray8"/>.</summary>
        void FromGray(Gray8 source)
        {
            FromRgb(source.ToRgb24());
        }

        /// <summary>Sets the pixel value from <see cref="Gray16"/>.</summary>
        void FromGray(Gray16 source)
        {
            FromRgb(source.ToRgb48());
        }

        /// <summary>Sets the pixel value from <see cref="GrayF"/>.</summary>
        void FromGray(GrayF source)
        {
            FromScaledVector3(source.ToScaledVector3());
        }

        /// <summary>Sets the pixel value from <see cref="GrayAlpha16"/>.</summary>
        void FromGrayAlpha(GrayAlpha16 source)
        {
            FromRgba(source.ToRgba32());
        }

        #endregion

        #region From Color

        /// <summary>Sets the pixel value from <see cref="Rgb24"/>.</summary>
        void FromRgb(Rgb24 source)
        {
            FromScaledVector(source.ToScaledVector3());
        }

        /// <summary>Sets the pixel value from <see cref="Rgb48"/>.</summary>
        void FromRgb(Rgb48 source)
        {
            FromScaledVector(source.ToScaledVector3());
        }

        /// <summary>Sets the pixel value from <see cref="Color"/>.</summary>
        void FromRgba(Color source)
        {
            FromScaledVector(source.ToScaledVector4());
        }

        /// <summary>Sets the pixel value from an <see cref="Rgba64"/>.</summary>
        void FromRgba(Rgba64 source)
        {
            FromScaledVector(source.ToScaledVector4());
        }

        #endregion

        #region From Arbitrary Color

        /// <summary>Sets the pixel value from an <see cref="Abgr32"/>.</summary>
        void FromAbgr(Abgr32 source)
        {
            FromRgba(source.ToRgba32());
        }

        /// <summary>Sets the pixel value from an <see cref="Argb32"/>.</summary>
        void FromArgb(Argb32 source)
        {
            FromRgba(source.ToRgba32());
        }

        /// <summary>Sets the pixel value from an <see cref="Bgr565"/>.</summary>
        void FromBgr(Bgr565 source)
        {
            FromBgr(source.ToBgr24());
        }

        /// <summary>Sets the pixel value from an <see cref="Bgr24"/>.</summary>
        void FromBgr(Bgr24 source)
        {
            FromRgb(source.ToRgb24());
        }

        /// <summary>Sets the pixel value from an <see cref="Bgra5551"/>.</summary>
        void FromBgra(Bgra5551 source)
        {
            FromRgba(source.ToRgba32());
        }

        /// <summary>Sets the pixel value from an <see cref="Bgra32"/>.</summary>
        void FromBgra(Bgra32 source)
        {
            FromRgba(source.ToRgba32());
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
        Gray8 ToGray8() => LuminanceHelper.BT709.ToGray8(ToRgb24());

        /// <summary>
        /// Gets a gray representation of the pixel as a 16-bit integer.
        /// </summary>
        Gray16 ToGray16() => LuminanceHelper.BT709.ToGray16(ToRgb48());

        /// <summary>
        /// Gets a gray and alpha representation of the pixel as a pair of 8-bit integers.
        /// </summary>
        GrayAlpha16 ToGrayAlpha16() => LuminanceHelper.BT709.ToGrayAlpha16(ToRgba32());

        /// <summary>
        /// Gets a gray representation of the pixel as a 32-bit float.
        /// </summary>
        GrayF ToGrayF() => LuminanceHelper.BT709.ToGrayF(ToRgbVector());

        #endregion

        #region To Color

        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 8-bit integer values.
        /// </summary>
        Rgb24 ToRgb24()
        {
            Rgb24 rgb = default; // TODO: Unsafe.SkipInit
            rgb.FromScaledVector(ToScaledVector3());
            return rgb;
        }

        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 16-bit integer values.
        /// </summary>
        Rgb48 ToRgb48()
        {
            Rgb48 rgb = default; // TODO: Unsafe.SkipInit
            rgb.FromScaledVector(ToScaledVector3());
            return rgb;
        }

        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 32-bit float values. 
        /// </summary>
        RgbVector ToRgbVector()
        {
            return ToScaledVector3();
        }

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha 8-bit integer values.
        /// </summary>
        Color ToRgba32()
        {
            Color rgba = default; // TODO: Unsafe.SkipInit
            rgba.FromScaledVector(ToScaledVector4());
            return rgba;
        }

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha 16-bit integer values.
        /// </summary>
        Rgba64 ToRgba64()
        {
            Rgba64 rgba = default; // TODO: Unsafe.SkipInit
            rgba.FromScaledVector(ToScaledVector4());
            return rgba;
        }

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha values 32-bit float values.
        /// </summary>
        RgbaVector ToRgbaVector()
        {
            return ToScaledVector4();
        }

        #endregion

        #region To Arbitrary Color

        /// <summary>
        /// Gets the pixel as a representation of blue, green, red, and alpha integer values.
        /// Red, green, and blue are 6-bit, alpha is 1-bit.
        /// </summary>
        Bgra5551 ToBgra5551()
        {
            Bgra5551 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromRgba(ToRgba32());
            return bgra;
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, and red 8-bit integer values.
        /// </summary>
        Bgr24 ToBgr24()
        {
            Bgr24 bgr = default; // TODO: Unsafe.SkipInit
            bgr.FromRgb(ToRgb24());
            return bgr;
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, and red integer values.
        /// Blue and red are 5-bit, green is 6-bit.
        /// </summary>
        Bgr565 ToBgr565()
        {
            Bgr565 bgr = default; // TODO: Unsafe.SkipInit
            bgr.FromRgb(ToRgb24());
            return bgr;
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, red, and alpha 8-bit integer values.
        /// </summary>
        Bgra32 ToBgra32()
        {
            Bgra32 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromRgba(ToRgba32());
            return bgra;
        }

        /// <summary>
        /// Gets the pixel as a representation of alpha, blue, green, and red 8-bit integer values.
        /// </summary>
        Abgr32 ToAbgr32()
        {
            Abgr32 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromRgba(ToRgba32());
            return bgra;
        }

        /// <summary>
        /// Gets the pixel as a representation of alpha, red, green, and blue 8-bit integer values.
        /// </summary>
        Argb32 ToArgb32()
        {
            Argb32 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromRgba(ToRgba32());
            return bgra;
        }

        #endregion
    }

    public interface IPixel<TSelf> : IPixel, IVector<TSelf>
        where TSelf : IPixel<TSelf>
    {
    }
}
