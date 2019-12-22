using System;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Base interface for objects that store pixels.
    /// </summary>
    public interface IPixelSource<TPixel> : IDisposable
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the width of the source in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the source in pixels.
        /// </summary>
        int Height { get; }
    }
}
