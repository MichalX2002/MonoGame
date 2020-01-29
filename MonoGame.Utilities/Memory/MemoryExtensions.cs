using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Memory
{
    public static class MemoryExtensions
    {
        public static bool IsEmpty(this IReadOnlyMemory memory)
        {
            if (memory == null)
                return true;
            return memory.ByteLength() == 0;
        }

        public static int ByteLength(this IReadOnlyMemory memory)
        {
            return memory.Length * memory.ElementSize;
        }
        
        public static unsafe ReadOnlySpan<byte> AsBytes(this IReadOnlyMemory memory)
        {
            var ptr = Unsafe.AsPointer(ref Unsafe.AsRef(memory.Data));
            return new ReadOnlySpan<byte>(ptr, memory.ByteLength());
        }

        public static unsafe Span<byte> AsBytes(this IMemory memory)
        {
            var ptr = Unsafe.AsPointer(ref memory.Data);
            return new Span<byte>(ptr, memory.ByteLength());
        }
    }
}
