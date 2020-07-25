using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace MonoGame.Framework
{
    public static partial class MathHelper
    {
        // TODO: optimize with ARM/NEON instructions

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static float SseClamp(float value, float min, float max)
        {
            // around 2x faster than managed (benchmarked on i7-4720HQ @ 2.6Ghz)
            var vals = Vector128.CreateScalarUnsafe(value);
            var mins = Vector128.CreateScalarUnsafe(min);
            var maxs = Vector128.CreateScalarUnsafe(max);
            return Sse.MaxScalar(mins, Sse.MinScalar(vals, maxs)).ToScalar();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static double Sse2Clamp(double value, double min, double max)
        {
            // around 2x faster than managed (benchmarked on i7-4720HQ @ 2.6Ghz)
            var vals = Vector128.CreateScalarUnsafe(value);
            var mins = Vector128.CreateScalarUnsafe(min);
            var maxs = Vector128.CreateScalarUnsafe(max);
            return Sse2.MaxScalar(mins, Sse2.MinScalar(vals, maxs)).ToScalar();
        }

        #region Clamp

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(float value, float min, float max)
        {
            if (Sse.IsSupported)
            {
                return SseClamp(value, min, max);
            }
            else
            {
                // First we check to see if we're greater than the max
                value = (value > max) ? max : value;

                // Then we check to see if we're less than the min.
                value = (value < min) ? min : value;

                // There's no check to see if min > max.
                return value;
            }
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (Sse2.IsSupported)
            {
                return Sse2Clamp(value, min, max);
            }
            else
            {
                // First we check to see if we're greater than the max
                value = (value > max) ? max : value;

                // Then we check to see if we're less than the min.
                value = (value < min) ? min : value;

                // There's no check to see if min > max.
                return value;
            }
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static byte Clamp(int value, byte min, byte max)
        {
            if (Sse.IsSupported)
            {
                return (byte)SseClamp(value, min, max);
            }
            else
            {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return (byte)value;
            }
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static short Clamp(int value, short min, short max)
        {
            if (Sse.IsSupported)
            {
                return (short)SseClamp(value, min, max);
            }
            else
            {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return (short)value;
            }
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static int Clamp(int value, int min, int max)
        {
            if (Sse2.IsSupported)
            {
                return (int)Sse2Clamp(value, min, max);
            }
            else
            {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return value;
            }
        }

        #endregion

        #region ClampTruncate

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static byte ClampTruncate(float value, byte min, byte max)
        {
            if (Sse.IsSupported)
            {
                return (byte)SseClamp(value, min, max);
            }
            else
            {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return (byte)value;
            }
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static short ClampTruncate(float value, short min, short max)
        {
            if (Sse.IsSupported)
            {
                return (short)SseClamp(value, min, max);
            }
            else
            {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return (short)value;
            }
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        [CLSCompliant(false)]
        public static ushort ClampTruncate(float value, ushort min, ushort max)
        {
            if (Sse.IsSupported)
            {
                return (ushort)SseClamp(value, min, max);
            }
            else
            {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return (ushort)value;
            }
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        [CLSCompliant(false)]
        public static uint ClampTruncate(double value, uint min, uint max)
        {
            if (Sse2.IsSupported)
            {
                return (uint)Sse2Clamp(value, min, max);
            }
            else
            {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return (uint)value;
            }
        }

        #endregion
    }
}
