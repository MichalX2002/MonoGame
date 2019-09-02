using System;

namespace MonoGame.Utilities.Memory
{
    public interface IMemory<T> : IDisposable
    {
        Span<T> Span { get; }
    }
}
