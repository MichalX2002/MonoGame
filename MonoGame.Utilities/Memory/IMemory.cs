using System;

namespace MonoGame.Framework.Memory
{
    public interface IMemory : IReadOnlyMemory
    {
        new ref byte Data { get; }
    }

    public interface IMemory<T> : IReadOnlyMemory<T>, IMemory
    {
        new Span<T> Span { get; }
    }
}
