using System;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelRowsProjector
    {
        internal static bool CheckBounds(this IReadOnlyPixelRowsContext context, Rectangle sourceRectangle)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Pixels.GetBounds() == sourceRectangle)
                return true;

            ImagingArgumentGuard.AssertRectangleInSource(context, sourceRectangle, nameof(sourceRectangle));
            return false;
        }
    }
}
