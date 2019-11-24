using System;

namespace MonoGame.Utilities.Memory
{
    public interface IReadOnlyMemory<T> : IDisposable
    {
        ReadOnlySpan<T> Span { get; }
    }
}
