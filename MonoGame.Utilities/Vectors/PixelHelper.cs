using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vectors
{
    public static class PixelHelper
    {
        public static Vector3 GrayFactor => LuminanceHelper.BT709.Factor;

        #region ToGray8

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToGray8(byte r, byte g, byte b)
        {
            return LuminanceHelper.BT709.ToGray8(r, g, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToGray8(Rgb24 rgb)
        {
            return ToGray8(rgb.R, rgb.G, rgb.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToGray8(Rgb48 rgb)
        {
            return ScalingHelper.ToUInt8(ToGray16(rgb));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToGray8<TPixel>(TPixel pixel)
            where TPixel : struct, IPixel
        {
            return ToGray8(pixel.ToRgb24());
        }

        #endregion

        #region ToGrayAlpha16

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GrayAlpha16 ToGrayAlpha16(byte r, byte g, byte b, byte a)
        {
            return LuminanceHelper.BT709.ToGrayAlpha16(r, g, b, a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GrayAlpha16 ToGrayAlpha16(byte r, byte g, byte b)
        {
            return ToGrayAlpha16(r, g, b, byte.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GrayAlpha16 ToGrayAlpha16(Color color)
        {
            return ToGrayAlpha16(color.R, color.G, color.B, color.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GrayAlpha16 ToGrayAlpha16<TPixel>(TPixel pixel)
            where TPixel : struct, IPixel
        {
            return ToGrayAlpha16(pixel.ToRgba32());
        }

        #endregion

        #region ToGray16

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToGray16(ushort r, ushort g, ushort b)
        {
            return LuminanceHelper.BT709.ToGray16(r, g, b);
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToGray16(Rgb48 rgb)
        {
            return ToGray16(rgb.R, rgb.G, rgb.B);
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToGray16<TPixel>(TPixel pixel)
            where TPixel : struct, IPixel
        {
            return ToGray16(pixel.ToRgb48());
        }

        #endregion

        #region ToGray32

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToGray32(uint r, uint g, uint b)
        {
            return LuminanceHelper.BT709.ToGray32(r, g, b);
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToGray32(Vector3 vector)
        {
            return LuminanceHelper.BT709.ToGray32(vector);
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToGray32<TPixel>(TPixel pixel)
            where TPixel : struct, IPixel
        {
            return ToGray32(pixel.ToScaledVector3());
        }

        #endregion


        #region ToGrayF

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToGrayF(Vector3 vector)
        {
            return LuminanceHelper.BT709.ToGrayF(vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToGrayF(float r, float g, float b)
        {
            return ToGrayF(new Vector3(r, g, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToGrayF<TPixel>(TPixel pixel)
            where TPixel : struct, IPixel
        {
            return ToGrayF(pixel.ToScaledVector3());
        }

        #endregion
    }
}
