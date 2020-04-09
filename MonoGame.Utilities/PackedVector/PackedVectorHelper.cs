using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.PackedVector
{
    public static class PackedVectorHelper
    {
        #region Component scaling

        // TODO: optimize some of these

        /// <summary>
        /// Scales a value from a 16-bit <see cref="ushort"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte DownScale16To8Bit(ushort component)
        {
            return (byte)(((component * 255) + 32895) >> 16);
        }

        /// <summary>
        /// Scales a value from an 32-bit <see cref="uint"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte DownScale32To8Bit(uint component)
        {
            return (byte)(component / (double)uint.MaxValue * byte.MaxValue);
        }

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 16-bit <see cref="ushort"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort UpScale8To16Bit(byte component)
        {
            return (ushort)(component * 257);
        }

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 32-bit <see cref="uint"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint UpScale8To32Bit(byte component)
        {
            return (uint)(component / (double)byte.MaxValue * uint.MaxValue);
        }

        /// <summary>
        /// Scales a value from an 16-bit <see cref="ushort"/> to it's 32-bit <see cref="uint"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint UpScale16To32Bit(ushort component)
        {
            return (uint)(component / (double)ushort.MaxValue * uint.MaxValue);
        }

        #endregion

        #region Luminance

        /// <summary>
        /// Gets the luminance from the RGB components using the formula specified by ITU-R Recommendation BT.709.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetBT709Luminance(float r, float g, float b)
        {
            return (r * .2126F) + (g * .7152F) + (b * .0722F);
        }

        /// <summary>
        /// Gets the luminance from the RGB components using the formula specified by ITU-R Recommendation BT.709.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Get8BitBT709Luminance(byte r, byte g, byte b)
        {
            return (byte)((r * .2126F) + (g * .7152F) + (b * .0722F) + 0.5f);
        }

        /// <summary>
        /// Gets the luminance from the RGB components using the formula specified by ITU-R Recommendation BT.709.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Get16BitBT709Luminance(ushort r, ushort g, ushort b)
        {
            return (ushort)((r * .2126F) + (g * .7152F) + (b * .0722F));
        }

        #endregion
    }
}
