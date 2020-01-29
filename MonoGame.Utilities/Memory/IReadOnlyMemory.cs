using System;

namespace MonoGame.Framework.Memory
{
    public interface IReadOnlyMemory : IDisposable
    {
        /// <summary>
        /// Gets the amount of elements contained.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the size of one element.
        /// </summary>
        int ElementSize { get; }

        ref readonly byte Data { get; }
    }

    public interface IReadOnlyMemory<T> : IReadOnlyMemory
    {
        ReadOnlySpan<T> Span { get; }
    }
}
