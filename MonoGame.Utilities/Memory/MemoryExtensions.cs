
namespace MonoGame.Framework.Memory
{
    public static class MemoryExtensions
    {
        public static bool IsEmpty<T>(this IReadOnlyMemory<T> memory)
        {
            if (memory == null)
                return true;
            return memory.Span.IsEmpty;
        }
    }
}
