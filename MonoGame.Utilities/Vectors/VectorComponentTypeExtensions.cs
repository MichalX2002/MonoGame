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

        public static string ToShortString(this VectorComponentType type)
        {
            return type switch
            {
                VectorComponentType.Int8 => "i8",
                VectorComponentType.UInt8 => "u8",

                VectorComponentType.Int16 => "i16",
                VectorComponentType.UInt16 => "u16",

                VectorComponentType.Int32 => "i32",
                VectorComponentType.UInt32 => "u32",

                VectorComponentType.Int64 => "i64",
                VectorComponentType.UInt64 => "u64",

                VectorComponentType.Float16 => "f16",
                VectorComponentType.Float32 => "f32",
                VectorComponentType.Float64 => "f64",

                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
    }
}
