using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    public static class VectorMaths
    {
        public static readonly Vector4 MaxBytes = new Vector4(byte.MaxValue);
        public static readonly Vector4 Half = new Vector4(0.5F);

        /// <summary>
        /// Scales a value from a 16 bit <see cref="ushort"/> to it's 8 bit <see cref="byte"/> equivalent.
        /// </summary>
        /// <param name="component">The 8 bit component value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte DownScale16To8Bit(ushort component)
        {
            return (byte)(((component * 255) + 32895) >> 16);
        }

        /// <summary>
        /// Scales a value from an 8 bit <see cref="byte"/> to it's 16 bit <see cref="ushort"/> equivalent.
        /// </summary>
        /// <param name="component">The 8 bit component value.</param>
        /// <returns>The <see cref="ushort"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort UpScale8To16Bit(byte component)
        {
            return (ushort)(component * 257);
        }

        /// <summary>
        /// Gets the luminance from the rgb components using the formula as specified by ITU-R Recommendation BT.709.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <returns>The <see cref="byte"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Get8BitBT709Luminance(byte r, byte g, byte b)
        {
            return (byte)((r * .2126F) + (g * .7152F) + (b * .0722F) + 0.5f);
        }

        /// <summary>
        /// Gets the luminance from the rgb components using the formula as specified by ITU-R Recommendation BT.709.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <returns>The <see cref="ushort"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Get16BitBT709Luminance(ushort r, ushort g, ushort b)
        {
            return (ushort)((r * .2126F) + (g * .7152F) + (b * .0722F));
        }

        /// <summary>
        /// Gets the luminance from the rgb components using the formula as specified by ITU-R Recommendation BT.709.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <returns>The <see cref="ushort"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Get16BitBT709Luminance(float r, float g, float b)
        {
            return (ushort)((r * .2126F) + (g * .7152F) + (b * .0722F));
        }
    }
}
