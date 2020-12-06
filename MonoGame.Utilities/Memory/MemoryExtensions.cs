using System;

namespace MonoGame.Framework.Memory
{
    public static class MemoryExtensions
    {
        public static int ByteLength(this IReadOnlyMemory memory)
        {
            if (memory == null)
                throw new ArgumentNullException(nameof(memory));
            return memory.Length * memory.ElementSize;
        }

        public static bool IsEmpty(this IReadOnlyMemory memory)
        {
            return memory.ByteLength() == 0;
        }
    }
}
