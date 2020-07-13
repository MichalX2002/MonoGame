using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vectors
{
    public static partial class LuminanceHelper
    {
        /// <summary>
        /// Converts RGB to luminance using the formula specified by ITU-R Recommendation BT.709.
        /// </summary>
        public static class BT709
        {
            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float ToGrayF(Vector3 rgb)
            {
                rgb *= new Vector3(.2126f, .7152f, .0722f);
                return rgb.X + rgb.Y + rgb.Z;
            }

            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float ToGrayF(float r, float g, float b)
            {
                return ToGrayF(new Vector3(r, g, b));
            }

            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static byte ToGray8(byte r, byte g, byte b)
            {
                return (byte)(ToGrayF(r, g, b) + 0.5f);
            }

            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static byte ToGray8(Rgb24 rgb)
            {
                return ToGray8(rgb.R, rgb.G, rgb.B);
            }

            /// <summary>
            /// Converts RGBA to luminance and alpha using the the BT.709 formula.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static GrayAlpha16 ToGrayAlpha16(Color rgba)
            {
                byte l = ToGray8(rgba.Rgb);
                return new GrayAlpha16(l, rgba.A);
            }

            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [CLSCompliant(false)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ushort ToGray16(ushort r, ushort g, ushort b)
            {
                return (ushort)(ToGrayF(r, g, b) + 0.5f);
            }

            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [CLSCompliant(false)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ushort ToGray16(Rgb48 rgb)
            {
                return ToGray16(rgb.R, rgb.G, rgb.B);
            }

            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [CLSCompliant(false)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static uint ToGray32(uint r, uint g, uint b)
            {
                return (uint)(ToGrayF(r, g, b) + 0.5f);
            }
        }
    }
}
