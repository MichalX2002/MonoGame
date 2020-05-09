using MonoGame.Framework.PackedVector;

namespace MonoGame.Framework
{
    public static class PackedVectorExtensions
    {
        public static Vector4 ToVector4<T>(this T pixel) where T : struct, IVector
        {
            pixel.ToVector4(out var vector);
            return vector;
        }

        public static Vector4 ToScaledVector4<T>(this T pixel) where T : struct, IVector
        {
            pixel.ToScaledVector4(out var vector);
            return vector;
        }
    }
}
