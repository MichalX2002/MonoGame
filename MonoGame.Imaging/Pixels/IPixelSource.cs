using System;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    /// <summary>
    /// Base interface for objects that store pixels.
    /// </summary>
    public interface IPixelSource : IDisposable
    {
        /// <summary>
        /// Gets the size of the source in pixels.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Gets info about the pixel type of the source.
        /// </summary>
        VectorTypeInfo PixelType { get; }
    }
}
