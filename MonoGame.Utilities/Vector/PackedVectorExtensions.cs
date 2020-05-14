using MonoGame.Framework.Vector;

namespace MonoGame.Framework
{
    public static class PackedVectorExtensions
    {
        public static void FromVector4<T>(this ref T pixel, Vector4 vector)
            where T : struct, IVector
        {
            pixel.FromVector4(vector);
        }

        public static Vector4 ToVector4<T>(this T pixel)
            where T : IVector
        {
            return pixel.ToVector4();
        }
    }
}
