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
        public static RgbVector ToRgbVector<T>(this T pixel)
            where T : struct, IPixel
        {
            return pixel.ToScaledVector3();
        }

        /// <summary>
        /// Gets the pixel as a representation of red, green, blue, and alpha values 32-bit float values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RgbaVector ToRgbaVector<T>(this T pixel)
            where T : struct, IPixel
        {
            return pixel.ToScaledVector4();
        }

        #region FromGray

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromGray<T>(this ref T destination, Gray8 source)
            where T : struct, IPixel
        {
            destination.FromGray(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromGray<T>(this ref T destination, Gray16 source)
            where T : struct, IPixel
        {
            destination.FromGray(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromGray<T>(this ref T destination, GrayF source)
            where T : struct, IPixel
        {
            destination.FromGray(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromGrayAlpha<T>(this ref T destination, GrayAlpha16 source)
            where T : struct, IPixel
        {
            destination.FromGrayAlpha(source);
        }

        #endregion

        #region FromRgb[a]

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromRgb<T>(this ref T destination, Rgb24 source)
            where T : struct, IPixel
        {
            destination.FromRgb(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromRgb<T>(this ref T destination, Rgb48 source)
            where T : struct, IPixel
        {
            destination.FromRgb(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromRgba<T>(this ref T destination, Color source)
            where T : struct, IPixel
        {
            destination.FromRgba(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromRgba<T>(this ref T destination, Rgba64 source)
            where T : struct, IPixel
        {
            destination.FromRgba(source);
        }

        #endregion

        #region To Arbitrary Color

        /// <summary>
        /// Gets the pixel as a representation of blue, green, red, and alpha integer values.
        /// Red, green, and blue are 6-bit, alpha is 1-bit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bgra5551 ToBgra5551<T>(this T source)
            where T : struct, IPixel
        {
            Bgra5551 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromRgba(source.ToRgba32());
            return bgra;
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, and red 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bgr24 ToBgr24<T>(this T source)
            where T : struct, IPixel
        {
            Bgr24 bgr = default; // TODO: Unsafe.SkipInit
            bgr.FromRgb(source.ToRgb24());
            return bgr;
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, and red integer values.
        /// Blue and red are 5-bit, green is 6-bit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bgr565 ToBgr565<T>(this T source)
            where T : struct, IPixel
        {
            Bgr565 bgr = default; // TODO: Unsafe.SkipInit
            bgr.FromRgb(source.ToRgb24());
            return bgr;
        }

        /// <summary>
        /// Gets the pixel as a representation of blue, green, red, and alpha 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bgra32 ToBgra32<T>(this T source)
            where T : struct, IPixel
        {
            Bgra32 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromRgba(source.ToRgba32());
            return bgra;
        }

        /// <summary>
        /// Gets the pixel as a representation of alpha, blue, green, and red 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Abgr32 ToAbgr32<T>(this T source)
            where T : struct, IPixel
        {
            Abgr32 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromRgba(source.ToRgba32());
            return bgra;
        }

        /// <summary>
        /// Gets the pixel as a representation of alpha, red, green, and blue 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Argb32 ToArgb32<T>(this T source)
            where T : struct, IPixel
        {
            Argb32 bgra = default; // TODO: Unsafe.SkipInit
            bgra.FromRgba(source.ToRgba32());
            return bgra;
        }

        #endregion
    }
}
