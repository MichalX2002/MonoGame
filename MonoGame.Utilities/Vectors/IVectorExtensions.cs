using System.Numerics;
using MonoGame.Framework.Vectors;

namespace MonoGame.Framework
{
    public static class IVectorExtensions
    {
        public static void FromVector4<T>(this ref T pixel, Vector4 vector)
            where T : struct, IVector
        {
            pixel.FromVector(vector);
        }

        public static Vector4 ToVector4<T>(this T pixel)
            where T : IVector
        {
            return pixel.ToVector4();
        }

        public static void FromVector3<T>(this ref T pixel, Vector3 vector)
            where T : struct, IVector
        {
            pixel.FromVector(vector);
        }

        public static Vector3 ToVector3<T>(this T pixel)
            where T : IVector
        {
            return pixel.ToScaledVector3();
        }
    }
}
