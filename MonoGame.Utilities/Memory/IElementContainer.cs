using System;

namespace MonoGame.Framework.Memory
{
    public interface IElementContainer : IDisposable
    {
        /// <summary>
        /// Gets the size of one element in bytes.
        /// </summary>
        int ElementSize { get; }

        /// <summary>
        /// Gets the amount of elements in the container.
        /// </summary>
        int Length { get; }
    }
}
