using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vectors
{
    public static class VectorComponentTypeExtensions
    {
        public static int SizeInBytes(this VectorComponentType type)
        {
            return type switch
            {
                VectorComponentType.Int8 => sizeof(sbyte),
                VectorComponentType.UInt8 => sizeof(byte),

                VectorComponentType.Int16 => sizeof(short),
                VectorComponentType.UInt16 => sizeof(ushort),

                VectorComponentType.Int32 => sizeof(int),
                VectorComponentType.UInt32 => sizeof(uint),

                VectorComponentType.Int64 => sizeof(long),
                VectorComponentType.UInt64 => sizeof(ulong),

                VectorComponentType.Float16 => Unsafe.SizeOf<HalfSingle>(),
                VectorComponentType.Float32 => sizeof(float),
                VectorComponentType.Float64 => sizeof(double),

                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
    }
}
