using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vectors
{
    public static class ScaledVectorHelper
    {
        #region ToScaledUInt

        // TODO: optimize with SSE (new intrinsics in NET5)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToScaledUInt8(Vector3 scaledVector)
        {
            scaledVector = VectorHelper.ScaledClamp(scaledVector);
            scaledVector *= byte.MaxValue;
            scaledVector += new Vector3(0.5f);

            return scaledVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToScaledUInt8(Vector4 scaledVector)
        {
            scaledVector = VectorHelper.ScaledClamp(scaledVector);
            scaledVector *= byte.MaxValue;
            scaledVector += new Vector4(0.5f);

            return scaledVector;
        }

        #endregion

        #region ToRgb[a]

        /// <summary>
        /// Gets the vector as a representation of red, green, and blue 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rgb24 ToRgb24<TVector>(TVector pixel)
            where TVector : struct, IVector
        {
            Rgb24 rgb = default; // TODO: Unsafe.SkipInit
            rgb.FromScaledVector(pixel.ToScaledVector3());
            return rgb;
        }

        /// <summary>
        /// Gets the vector as a representation of red, green, and blue 16-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rgb48 ToRgb48<TVector>(TVector pixel)
            where TVector : struct, IVector
        {
            Rgb48 rgb = default; // TODO: Unsafe.SkipInit
            rgb.FromScaledVector(pixel.ToScaledVector3());
            return rgb;
        }

        /// <summary>
        /// Gets the vector as a representation of red, green, blue, and alpha 8-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToRgba32<TVector>(TVector pixel)
            where TVector : struct, IVector
        {
            Color rgba = default; // TODO: Unsafe.SkipInit
            rgba.FromScaledVector(pixel.ToScaledVector4());
            return rgba;
        }

        /// <summary>
        /// Gets the vector as a representation of red, green, blue, and alpha 16-bit integer values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rgba64 ToRgba64<TVector>(TVector pixel)
            where TVector : struct, IVector
        {
            Rgba64 rgba = default; // TODO: Unsafe.SkipInit
            rgba.FromScaledVector(pixel.ToScaledVector4());
            return rgba;
        }

        #endregion
    }
}