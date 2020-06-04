using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework
{
    public static class Vector3Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref this Vector3 vector, Vector3 min, Vector3 max)
        {
            vector = Vector3.Clamp(vector, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref this Vector3 vector, float min, float max)
        {
            vector = Vector3.Clamp(vector, new Vector3(min), new Vector3(max));
        }

        public static void Round(ref this Vector3 vector)
        {
            vector.X = MathF.Round(vector.X);
            vector.Y = MathF.Round(vector.Y);
            vector.Z = MathF.Round(vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(in this Vector3 vector)
        {
            return UnsafeR.As<Vector3, Vector2>(vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(in this Vector3 vector)
        {
            return new Vector4(vector, 1);
        }
    }
}
