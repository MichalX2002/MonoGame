using System;
using MonoGame.Framework.Vectors;

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
        VectorType PixelType { get; }
    }
}
