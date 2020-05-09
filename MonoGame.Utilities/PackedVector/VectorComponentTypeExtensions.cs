using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vector
{
    public static class VectorComponentTypeExtensions
    {
        public static int SizeInBytes(this VectorComponentType type)
        {
            return type switch
            {
                VectorComponentType.Int8 => sizeof(byte),
                VectorComponentType.Int16 => sizeof(short),
                VectorComponentType.Int32 => sizeof(int),
                VectorComponentType.Int64 => sizeof(long),

                VectorComponentType.Float16 => Unsafe.SizeOf<HalfSingle>(),
                VectorComponentType.Float32 => sizeof(float),
                VectorComponentType.Float64 => sizeof(double),

                _ => 0,
            };
        }
    }
}
