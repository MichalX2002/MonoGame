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

        private const string ErrorMessage = "Did you mean to use the value directly?";

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
        public static float ToFloat32FromUInt4(byte component)
        {
            return component / UInt4Max;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32FromUInt5(byte component)
        {
            return component / UInt5Max;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32FromUInt6(byte component)
        {
            return component / UInt6Max;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32(byte component)
        {
            return component / (float)byte.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32(ushort component)
        {
            return component / (float)ushort.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static double ToFloat64(uint component)
        {
            return component / (double)uint.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static float ToFloat32(uint component)
        {
            return (float)ToFloat64(component);
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
        public static byte ToUInt8(float component)
        {
            component *= byte.MaxValue;
            component += 0.5f;
            return MathHelper.ClampTruncate(component, byte.MinValue, byte.MaxValue);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static ushort ToUInt16(float component)
        {
            component *= ushort.MaxValue;
            component += 0.5f;
            return MathHelper.ClampTruncate(component, ushort.MinValue, ushort.MaxValue);
        }

        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static uint ToUInt32(double component)
        {
            component *= uint.MaxValue;
            component += 0.5f;
            return MathHelper.ClampTruncate(component, uint.MinValue, uint.MaxValue);
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
            return (byte)(value / 1024);
        }

        #endregion

        #region UInt8

        /// <summary>
        /// Scales a value from a 16-bit <see cref="ushort"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt8(ushort component)
        {
            return (byte)(((component * 255) + 32895) >> 16);
        }

        /// <summary>
        /// Scales a value from an 32-bit <see cref="uint"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static byte ToUInt8(uint component)
        {
            return (byte)(component / (double)uint.MaxValue * byte.MaxValue);
        }

        #endregion

        #region UInt16

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 16-bit <see cref="ushort"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static ushort ToUInt16(byte component)
        {
            return (ushort)(component * (ushort.MaxValue / byte.MaxValue));
        }

        #endregion

        #region UInt32

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 32-bit <see cref="uint"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static uint ToUInt32(byte component)
        {
            return component * (uint.MaxValue / byte.MaxValue);
        }

        /// <summary>
        /// Scales a value from an 16-bit <see cref="ushort"/> to it's 32-bit <see cref="uint"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(ImplOptions)]
        public static uint ToUInt32(ushort component)
        {
            return component * (uint.MaxValue / ushort.MaxValue);
        }

        #endregion

        #endregion
    }
}
