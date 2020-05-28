using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vector
{
    public static class VectorHelper
    {
        // TODO: optimize some of these

        #region "Error catchers"

        private const string ErrorMessage = "Did you mean to use the value directly?";

        [CLSCompliant(false)]
        [Obsolete(ErrorMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToUInt8(byte value)
        {
            return value;
        }

        [CLSCompliant(false)]
        [Obsolete(ErrorMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUInt16(ushort value)
        {
            return value;
        }

        [CLSCompliant(false)]
        [Obsolete(ErrorMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(uint value)
        {
            return value;
        }

        [Obsolete(ErrorMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat32(float value)
        {
            return value;
        }

        [Obsolete(ErrorMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToFloat64(double value)
        {
            return value;
        }

        #endregion

        #region Float scaling

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat32(byte component)
        {
            return component / (float)byte.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat32(ushort component)
        {
            return component / (float)ushort.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToFloat64(uint component)
        {
            return component / (double)uint.MaxValue;
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat32(uint component)
        {
            return (float)ToFloat64(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToUInt8(float component)
        {
            component *= byte.MaxValue;
            component += 0.5f;
            return MathHelper.ClampTruncate(component, byte.MinValue, byte.MaxValue);
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUInt16(float component)
        {
            component *= ushort.MaxValue;
            component += 0.5f;
            return MathHelper.ClampTruncate(component, ushort.MinValue, ushort.MaxValue);
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(double component)
        {
            component *= uint.MaxValue;
            component += 0.5f;
            return MathHelper.ClampTruncate(component, uint.MinValue, uint.MaxValue);
        }

        #endregion

        #region Integer scaling

        /// <summary>
        /// Scales a value from a 16-bit <see cref="ushort"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToUInt8(ushort component)
        {
            return (byte)(((component * 255) + 32895) >> 16);
        }

        /// <summary>
        /// Scales a value from an 32-bit <see cref="uint"/> to it's 8-bit <see cref="byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToUInt8(uint component)
        {
            return (byte)(component / (double)uint.MaxValue * byte.MaxValue);
        }

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 16-bit <see cref="ushort"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUInt16(byte component)
        {
            return (ushort)(component * 257);
        }

        /// <summary>
        /// Scales a value from an 8-bit <see cref="byte"/> to it's 32-bit <see cref="uint"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(byte component)
        {
            return (uint)(component / (float)byte.MaxValue * uint.MaxValue);
        }

        /// <summary>
        /// Scales a value from an 16-bit <see cref="ushort"/> to it's 32-bit <see cref="uint"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(ushort component)
        {
            return (uint)(component / (float)ushort.MaxValue * uint.MaxValue);
        }

        #endregion
    }
}
