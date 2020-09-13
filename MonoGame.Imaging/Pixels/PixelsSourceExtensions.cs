using System;
using MonoGame.Framework;

namespace MonoGame.Imaging.Pixels
{
    public static class PixelSourceExtensions
    {
        /// <summary>
        /// Gets the width and height of the pixel source as a <see cref="Size"/>.
        /// </summary>
        public static Size GetSize(this IPixelSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Size(source.Width, source.Height);
        }

        /// <summary>
        /// Gets the width and height of the pixel source as a <see cref="Rectangle"/>.
        /// </summary>
        /// <returns>The buffer bounds at point (0, 0).</returns>
        public static Rectangle GetBounds(this IPixelSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Rectangle(Point.Zero, source.GetSize());
        }
    }
}
