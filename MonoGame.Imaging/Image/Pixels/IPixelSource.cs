using System;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Base interface for objects that store pixels.
    /// </summary>
    public interface IPixelSource : IDisposable
    {
        /// <summary>
        /// Gets the width of the source in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the source in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets info about the pixel type of the source.
        /// </summary>
        PixelTypeInfo PixelType { get; }
    }

    /// <summary>
    /// Base interface for objects that store a defined type of pixels.
    /// </summary>
    /// <typeparam name="TPixel">The type of pixels that the source stores.</typeparam>
    public interface IPixelSource<TPixel> : IPixelSource
        where TPixel : unmanaged, IPixel
    {
    }
}
