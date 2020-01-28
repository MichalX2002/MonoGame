using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.PackedVector
{
    public static class PackedVectorHelper
    {
        /// <summary>
        /// Scales a value from a 16-bit <see cref="ushort"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte DownScale16To8Bit(ushort component) =>
            (byte)(((component * 255) + 32895) >> 16);

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 16-bit <see cref="ushort"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort UpScale8To16Bit(byte component) => (ushort)(component * 257);

        /// <summary>
        /// Gets the luminance from the RGB components using the formula specified by ITU-R Recommendation BT.709.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Get8BitBT709Luminance(byte r, byte g, byte b) =>
            (byte)((r * .2126F) + (g * .7152F) + (b * .0722F) + 0.5f);

        /// <summary>
        /// Gets the luminance from the RGB components using the formula specified by ITU-R Recommendation BT.709.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Get16BitBT709Luminance(ushort r, ushort g, ushort b) => 
            (ushort)((r * .2126F) + (g * .7152F) + (b * .0722F));

        /// <summary>
        /// Gets the luminance from the RGB components using the formula specified by ITU-R Recommendation BT.709.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get32BitBT709Luminance(uint r, uint g, uint b) =>
            (uint)((r * .2126F) + (g * .7152F) + (b * .0722F));

        /// <summary>
        /// Gets the luminance from the RGB components using the formula specified by ITU-R Recommendation BT.709.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Get16BitBT709Luminance(float r, float g, float b) => 
            (ushort)((r * .2126F) + (g * .7152F) + (b * .0722F));
    }
}
