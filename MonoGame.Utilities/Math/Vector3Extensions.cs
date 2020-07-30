using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework
{
    public static class Vector3Extensions
    {
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
