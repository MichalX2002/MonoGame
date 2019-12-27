using System;

namespace MonoGame.Framework.Memory
{
    public interface IReadOnlyMemory<T> : IDisposable
    {
        ReadOnlySpan<T> Span { get; }
    }
}
