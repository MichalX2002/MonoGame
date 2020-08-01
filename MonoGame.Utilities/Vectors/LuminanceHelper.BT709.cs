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
            public static Vector3 Factor => new Vector3(.2126f, .7152f, .0722f);

            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float ToGrayF(Vector3 vector)
            {
                vector *= Factor;
                return vector.X + vector.Y + vector.Z;
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
            /// Converts RGBA to luminance and alpha using the the BT.709 formula.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static GrayAlpha16 ToGrayAlpha16(byte r, byte g, byte b, byte a)
            {
                byte l = ToGray8(r, g, b);
                return new GrayAlpha16(l, a);
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
            public static uint ToGray32(uint r, uint g, uint b)
            {
                return MathHelper.ClampTruncate(ToGrayF(r, g, b) + 0.5f, uint.MinValue, uint.MaxValue);
            }

            /// <summary>
            /// Converts RGB to luminance using the the BT.709 formula.
            /// </summary>
            [CLSCompliant(false)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static uint ToGray32(Vector3 vector)
            {
                return MathHelper.ClampTruncate(ToGrayF(vector) + 0.5f, uint.MinValue, uint.MaxValue);
            }
        }
    }
}
