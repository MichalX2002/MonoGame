using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework
{
    public static class Vector4Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref this Vector4 vector, Vector4 min, Vector4 max)
        {
            vector = Vector4.Clamp(vector, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref this Vector4 vector, float min, float max)
        {
            vector = Vector4.Clamp(vector, new Vector4(min), new Vector4(max));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(ref this Vector4 vector)
        {
            vector = Vector4.Normalize(vector);
        }

        public static void Round(ref this Vector4 vector)
        {
            vector.X = MathF.Round(vector.X);
            vector.Y = MathF.Round(vector.Y);
            vector.Z = MathF.Round(vector.Z);
            vector.W = MathF.Round(vector.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(in this Vector4 vector)
        {
            return UnsafeR.As<Vector4, Vector2>(vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(in this Vector4 vector)
        {
            return UnsafeR.As<Vector4, Vector3>(vector);
        }
    }
}
