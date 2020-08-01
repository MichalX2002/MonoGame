// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Vectors;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Assorted helpers for doing useful things with bitmaps.
    /// </summary>
    internal static class BitmapUtils
    {
        /// <summary>
        /// Checks whether an area of a bitmap contains entirely the specified alpha value.
        /// </summary>
        /// <param name="expectedAlpha"></param>
        /// <param name="bitmap"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static bool IsAlphaEntirely(byte expectedAlpha, BitmapContent bitmap, Rectangle? region = null)
        {
            var bitmapRegion = region ?? new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            if (bitmap is PixelBitmapContent<Alpha8> alphaBmp)
            {
                for (int y = 0; y < bitmapRegion.Height; y++)
                {
                    var row = alphaBmp.GetRowSpan(bitmapRegion.Y + y);
                    for (int x = 0; x < bitmapRegion.Width; x++)
                    {
                        if (row[bitmapRegion.X + x].A != expectedAlpha)
                            return false;
                    }
                }
                return true;
            }
            else if (bitmap is PixelBitmapContent<Color> rgbaBmp)
            {
                for (int y = 0; y < bitmapRegion.Height; y++)
                {
                    var row = rgbaBmp.GetRowSpan(bitmapRegion.Y + y);
                    for (int x = 0; x < bitmapRegion.Width; x++)
                    {
                        if (row[bitmapRegion.X + x].A != expectedAlpha)
                            return false;
                    }
                }
                return true;
            }
            else
            {
                throw new ArgumentException(
                    "Expected PixelBitmapContent<Alpha8> or PixelBitmapContent<Rgba32> but got " +
                    bitmap.GetType().Name, nameof(bitmap));
            }
        }
    }
}
