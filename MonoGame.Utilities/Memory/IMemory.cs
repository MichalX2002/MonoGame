using System;

namespace MonoGame.Utilities.Memory
{
    public interface IMemory<T> : IReadOnlyMemory<T>
    {
        new Span<T> Span { get; }
    }
}
