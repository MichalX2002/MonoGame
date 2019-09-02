using System;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Acts as a base interface for all objects that store pixels.
    /// </summary>
    public interface IPixelSource<TPixel> : IDisposable
        where TPixel : unmanaged, IPixel
    {
        /// <summary>
        /// Gets the width of the view in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the view in pixels.
        /// </summary>
        int Height { get; }
    }
}
