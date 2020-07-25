using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework
{
    public static class Vector4Extensions
    {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToScaledVector3(in this Vector4 vector)
        {
            return vector.ToVector3();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToScaledVector4(in this Vector4 vector)
        {
            return vector;
        }
    }
}
