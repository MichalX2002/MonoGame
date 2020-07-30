using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework
{
    public static class Vector2Extensions
    {
        public static PointF ToPointF(in this Vector2 vector)
        {
            return UnsafeR.As<Vector2, PointF>(vector);
        }

        public static SizeF ToSizeF(in this Vector2 vector)
        {
            return UnsafeR.As<Vector2, SizeF>(vector);
        }

        public static Point ToPoint(this Vector2 vector)
        {
            vector += new Vector2(0.5f);
            return new Point((int)vector.X, (int)vector.Y);
        }

        public static Size ToSize(this Vector2 vector)
        {
            vector += new Vector2(0.5f);
            return new Size((int)vector.X, (int)vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(in this Vector2 vector)
        {
            return new Vector3(vector, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(in this Vector2 vector)
        {
            return new Vector4(vector, 0, 1);
        }
    }
}
