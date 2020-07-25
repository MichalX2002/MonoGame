using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vectors
{
    public static class ScalingHelper
    {
        private const MethodImplOptions ImplOptions = 
            MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;

        private const float UInt4Max = 0b1111;
        private const float UInt5Max = 0b1_1111;
        private const float UInt6Max = 0b11_1111;

        // TODO: optimize some of these

        #region "Error catchers"

        private const string ErrorMessage = 
            "This method does nothing with the passed value. Did you mean to use the value directly?";

        [Obsolete(ErrorMessage)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt8(byte value)
        {
            return value;
        }

        [CLSCompliant(false)]
        [Obsolete(ErrorMessage)]
        [MethodImpl(ImplOptions)]
        public static ushort ToUInt16(ushort value)
        {
            return value;
        }

        [CLSCompliant(false)]
        [Obsolete(ErrorMessage)]
        [MethodImpl(ImplOptions)]
        public static uint ToUInt32(uint value)
        {
            return value;
        }

        [Obsolete(ErrorMessage)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32(float value)
        {
            return value;
        }

        [Obsolete(ErrorMessage)]
        [MethodImpl(ImplOptions)]
        public static double ToFloat64(double value)
        {
            return value;
        }

        #endregion

        #region Integer to Float scaling

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32FromUInt4(byte value)
        {
            return value / UInt4Max;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32FromUInt5(byte value)
        {
            return value / UInt5Max;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32FromUInt6(byte value)
        {
            return value / UInt6Max;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32(byte value)
        {
            return value / (float)byte.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32(ushort value)
        {
            return value / (float)ushort.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32(short value)
        {
            return ToFloat32(ToUInt16(value));
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static double ToFloat64(uint value)
        {
            return value / (double)uint.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32(uint value)
        {
            return (float)ToFloat64(value);
        }

        #endregion

        #region Float to Integer scaling

        [MethodImpl(ImplOptions)]
        public static byte ToUInt4(float value)
        {
            return (byte)(value * UInt4Max + 0.5f);
        }

        [MethodImpl(ImplOptions)]
        public static byte ToUInt5(float value)
        {
            return (byte)(value * UInt5Max + 0.5f);
        }

        [MethodImpl(ImplOptions)]
        public static byte ToUInt6(float value)
        {
            return (byte)(value * UInt6Max + 0.5f);
        }

        [MethodImpl(ImplOptions)]
        public static byte ToUInt8(float value)
        {
            value *= byte.MaxValue;
            value += 0.5f;
            return MathHelper.ClampTruncate(value, byte.MinValue, byte.MaxValue);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static short ToInt16(float value)
        {
            value = MathHelper.Clamp(value, 0, 1);
            value *= ushort.MaxValue;
            value += short.MinValue;
            return (short)MathF.Round(value);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static ushort ToUInt16(float value)
        {
            value *= ushort.MaxValue;
            value += 0.5f;
            return MathHelper.ClampTruncate(value, ushort.MinValue, ushort.MaxValue);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static uint ToUInt32(double value)
        {
            value *= uint.MaxValue;
            value += 0.5f;
            return MathHelper.ClampTruncate(value, uint.MinValue, uint.MaxValue);
        }

        #endregion

        #region Integer scaling

        #region UInt4

        [MethodImpl(ImplOptions)]
        public static byte ToUInt4(byte value)
        {
            return (byte)(value / 16);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt4(ushort value)
        {
            return (byte)(value / 4096);
        }

        #endregion

        #region UInt5

        [MethodImpl(ImplOptions)]
        public static byte ToUInt5(byte value)
        {
            return (byte)(value / 8);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt5(ushort value)
        {
            return (byte)(value / 2048);
        }

        #endregion

        #region UInt6

        [MethodImpl(ImplOptions)]
        public static byte ToUInt6(byte value)
        {
            return (byte)(value / 4);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt6(ushort value)
        {
            return (byte)(value / 1040);
        }

        #endregion

        #region UInt8

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt8From4(byte value)
        {
            return (byte)(value * 17);
        }

        /// <summary>
        /// Scales a value from a 16-bit <see cref="ushort"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt8(ushort value)
        {
            return (byte)(((value * 255) + 32895) >> 16);
        }

        [MethodImpl(ImplOptions)]
        public static byte ToUInt8(short value)
        {
            return ToUInt8(ToUInt16(value));
        }

        /// <summary>
        /// Scales a value from an 32-bit <see cref="uint"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt8(uint value)
        {
            return (byte)(value / 16843009);
        }

        #endregion

        #region UInt16
        
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static ushort ToUInt16From4(byte value)
        {
            return (ushort)(value * 4369);
        }

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 16-bit <see cref="ushort"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static ushort ToUInt16(byte value)
        {
            return (ushort)(value * 257);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static ushort ToUInt16(short value)
        {
            return (ushort)(value - short.MinValue);
        }

        #endregion

        #region UInt32

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 32-bit <see cref="uint"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static uint ToUInt32(byte value)
        {
            return value * 16843009u;
        }

        /// <summary>
        /// Scales a value from an 16-bit <see cref="ushort"/> to it's 32-bit <see cref="uint"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static uint ToUInt32(ushort value)
        {
            return value * 65537u;
        }

        #endregion

        #endregion
    }
}
