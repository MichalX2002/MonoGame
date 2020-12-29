using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework
{
    public static class UnsafeR
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo As<TFrom, TTo>(in TFrom source)
        {
            return ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(source));
        }

        public static ref readonly T Add<T>(in T source, nint offset)
        {
            return ref Unsafe.Add(ref Unsafe.AsRef(source), offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T AddByteOffset<T>(in T source, nint byteOffset)
        {
            return ref Unsafe.AddByteOffset(ref Unsafe.AsRef(source), byteOffset);
        }

        public static ReadOnlySpan<T> AsReadOnlySpan<T>(in T value, int count = 1)
        {
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(value), count);
        }

        public static Span<T> AsSpan<T>(ref T value, int count = 1)
        {
            return MemoryMarshal.CreateSpan(ref value, count);
        }
    }
}
