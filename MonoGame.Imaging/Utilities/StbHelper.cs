using System.Runtime.CompilerServices;
using MonoGame.Framework;
using StbSharp;

namespace MonoGame.Imaging.Utilities
{
    public static class StbHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToRectangle(this Rect rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.W, rect.H);
        }

        public static Rectangle? ToRectangle(this Rect? rect)
        {
            if (rect.HasValue)
                return ToRectangle(rect.Value);

            return default;
        }
    }
}
