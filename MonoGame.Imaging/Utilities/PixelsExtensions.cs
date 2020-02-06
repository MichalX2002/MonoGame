using System;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public static class PixelBufferExtensions
    {
        /// <summary>
        /// Gets the width and height of the source as a <see cref="Size"/>.
        /// </summary>
        /// <returns>The buffer size in pixels.</returns>
        public static Size GetSize(this IPixelSource source)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));
            return new Size(source.Width, source.Height);
        }

        /// <summary>
        /// Gets the width and height of the source as a <see cref="Rectangle"/>.
        /// </summary>
        /// <returns>The buffer bounds starting at point (0, 0).</returns>
        public static Rectangle GetBounds(this IPixelSource source)
        {
            return new Rectangle(Point.Zero, GetSize(source));
        }
    }
}
