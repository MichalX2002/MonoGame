
namespace MonoGame.Utilities.Memory
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
