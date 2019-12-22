using System;

namespace MonoGame.Framework.Memory
{
    public interface IMemory<T> : IReadOnlyMemory<T>
    {
        new Span<T> Span { get; }
    }
}
