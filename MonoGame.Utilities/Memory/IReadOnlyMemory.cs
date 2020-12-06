using System;

namespace MonoGame.Framework.Memory
{
    public interface IReadOnlyMemory : IElementContainer
    {
        ReadOnlySpan<byte> ByteSpan { get; }
    }

    public interface IReadOnlyMemory<T> : IReadOnlyMemory
    {
        ReadOnlySpan<T> Span { get; }
    }
}
