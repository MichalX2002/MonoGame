using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    public static class PackedVectorHelper
    {
        /// <summary>
        /// <see cref="Vector4"/> with all values set to <see cref="byte.MaxValue"/>.
        /// </summary>
        public static readonly Vector4 MaxBytes = new Vector4(byte.MaxValue);

        /// <summary>
        /// <see cref="Vector4"/> with all values set to <value>0.5f</value>.
        /// </summary>
        public static readonly Vector4 Half = new Vector4(0.5F);

        /// <summary>
        /// Gets the packed vector in <see cref="Vector3"/> format.
        /// </summary>
        public static Vector3 ToVector3<TPackedVector>(this TPackedVector vector)
            where TPackedVector : IPackedVector
        {
            return vector.ToVector4().ToVector3();
        }

        /// <summary>
        /// Scales a value from a 16 bit <see cref="ushort"/> to it's 8 bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte DownScale16To8Bit(ushort component)
        {
            return (byte)(((component * 255) + 32895) >> 16);
        }

        /// <summary>
        /// Scales a value from an 8 bit <see cref="byte"/> to it's 16 bit <see cref="ushort"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort UpScale8To16Bit(byte component)
        {
            return (ushort)(component * 257);
        }

        /// <summary>
        /// Gets the luminance from the RGB components using the formula as specified by ITU-R Recommendation BT.709.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Get8BitBT709Luminance(byte r, byte g, byte b)
        {
            return (byte)((r * .2126F) + (g * .7152F) + (b * .0722F) + 0.5f);
        }

        /// <summary>
        /// Gets the luminance from the RGB components using the formula as specified by ITU-R Recommendation BT.709.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Get16BitBT709Luminance(ushort r, ushort g, ushort b)
        {
            return (ushort)((r * .2126F) + (g * .7152F) + (b * .0722F));
        }

        /// <summary>
        /// Gets the luminance from the RGB components using the formula as specified by ITU-R Recommendation BT.709.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetBT709Luminance(float r, float g, float b)
        {
            return (r * .2126F) + (g * .7152F) + (b * .0722F);
        }

        /// <summary>
        /// Gets the luminance from the RGB components using the formula as specified by ITU-R Recommendation BT.709.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Get16BitBT709Luminance(float r, float g, float b)
        {
            return (ushort)((r * .2126F) + (g * .7152F) + (b * .0722F));
        }
    }
}
