using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Provides extension methods that expose the default implementation of
    /// some <see cref="IPixel"/> methods.
    /// </summary>
    [CLSCompliant(false)]
    public static class IPixelExtensions
    {
        /// <summary>
        /// Gets the pixel as a representation of red, green, and blue 32-bit float values. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RgbVector ToRgbVector<TPixel>(this TPixel pixel)
            where TPixel : struct, IPixel
        {
            return pixel.ToScaledVector3();
        }

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha values 32-bit float values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RgbaVector ToRgbaVector<TPixel>(this TPixel pixel)
            where TPixel : struct, IPixel
        {
            return pixel.ToScaledVector4();
        }

        #region FromGray

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromGray<TPixel>(this ref TPixel destination, Gray8 source)
            where TPixel : struct, IPixel
        {
            destination.FromGray(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromGray<TPixel>(this ref TPixel destination, Gray16 source)
            where TPixel : struct, IPixel
        {
            destination.FromGray(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromGray<TPixel>(this ref TPixel destination, GrayF source)
            where TPixel : struct, IPixel
        {
            destination.FromGray(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromGray<TPixel>(this ref TPixel destination, GrayAlpha16 source)
            where TPixel : struct, IPixel
        {
            destination.FromGray(source);
        }

        #endregion

        #region FromColor

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromColor<TPixel>(this ref TPixel destination, Rgb24 source)
            where TPixel : struct, IPixel
        {
            destination.FromColor(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromColor<TPixel>(this ref TPixel destination, Rgb48 source)
            where TPixel : struct, IPixel
        {
            destination.FromColor(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromColor<TPixel>(this ref TPixel destination, Color source)
            where TPixel : struct, IPixel
        {
            destination.FromColor(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromColor<TPixel>(this ref TPixel destination, Rgba64 source)
            where TPixel : struct, IPixel
        {
            destination.FromColor(source);
        }

        #endregion

        #region To Arbitrary Color

        /// <summary>
        /// Gets the pixel as a representation of blue, green, red, and alpha integer values.
        /// Red, green, and blue are 6-bit, alpha is 1-bit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bgra5551 ToBgra5551<TPixel>(this TPixel source)
            where TPixel : struct, IPixel
        {
            Bgra5551 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromColor(source.ToRgba32());
            return bgra;
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, and red integer values.
        /// Blue and red are 5-bit, green is 6-bit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bgr565 ToBgr565<TPixel>(this TPixel source)
            where TPixel : struct, IPixel
        {
            Bgr565 bgr = default; // TODO: Unsafe.SkipInit
            bgr.FromColor(source.ToRgb24());
            return bgr;
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, and red 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bgr24 ToBgr24<TPixel>(this TPixel source)
            where TPixel : struct, IPixel
        {
            Rgb24 rgb = source.ToRgb24();
            return new Bgr24(rgb.R, rgb.G, rgb.B);
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, red, and alpha 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bgra32 ToBgra32<TPixel>(this TPixel source)
            where TPixel : struct, IPixel
        {
            Color rgba = source.ToRgba32();
            return new Bgra32(rgba.R, rgba.G, rgba.B, rgba.A);
        }

        /// <summary>
        /// Gets the pixel as a representation of alpha, blue, green, and red 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Abgr32 ToAbgr32<TPixel>(this TPixel source)
            where TPixel : struct, IPixel
        {
            Color rgba = source.ToRgba32();
            return new Abgr32(rgba.R, rgba.G, rgba.B, rgba.A);
        }

        /// <summary>
        /// Gets the pixel as a representation of alpha, red, green, and blue 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Argb32 ToArgb32<TPixel>(this TPixel source)
            where TPixel : struct, IPixel
        {
            Color rgba = source.ToRgba32();
            return new Argb32(rgba.R, rgba.G, rgba.B, rgba.A);
        }

        #endregion
    }
}
