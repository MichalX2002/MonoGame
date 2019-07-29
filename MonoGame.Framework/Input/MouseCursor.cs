// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// Describes a mouse cursor.
    /// </summary>
    public partial class MouseCursor : IDisposable
    {
        #region Predefined Cursors

        /// <summary>
        /// Gets the default arrow cursor.
        /// </summary>
        public static MouseCursor Arrow { get; private set; }

        /// <summary>
        /// Gets the cursor that appears when the mouse is over text editing regions.
        /// </summary>
        public static MouseCursor IBeam { get; private set; }

        /// <summary>
        /// Gets the waiting cursor that appears while the application/system is busy.
        /// </summary>
        public static MouseCursor Wait { get; private set; }

        /// <summary>
        /// Gets the crosshair ("+") cursor.
        /// </summary>
        public static MouseCursor Crosshair { get; private set; }

        /// <summary>
        /// Gets the cross between Arrow and Wait cursors.
        /// </summary>
        public static MouseCursor WaitArrow { get; private set; }

        /// <summary>
        /// Gets the northwest/southeast ("\") cursor.
        /// </summary>
        public static MouseCursor SizeNWSE { get; private set; }

        /// <summary>
        /// Gets the northeast/southwest ("/") cursor.
        /// </summary>
        public static MouseCursor SizeNESW { get; private set; }

        /// <summary>
        /// Gets the horizontal west/east ("-") cursor.
        /// </summary>
        public static MouseCursor SizeWE { get; private set; }

        /// <summary>
        /// Gets the vertical north/south ("|") cursor.
        /// </summary>
        public static MouseCursor SizeNS { get; private set; }

        /// <summary>
        /// Gets the size all cursor which points in all directions.
        /// </summary>
        public static MouseCursor SizeAll { get; private set; }

        /// <summary>
        /// Gets the cursor that points that something is invalid, usually a cross.
        /// </summary>
        public static MouseCursor No { get; private set; }

        /// <summary>
        /// Gets the hand cursor, usually used for web links.
        /// </summary>
        public static MouseCursor Hand { get; private set; }

        #endregion

        private bool _disposed;

        public IntPtr Handle { get; private set; }

        /// <summary>
        /// Creates a mouse cursor from the specified texture.
        /// </summary>
        /// <param name="texture">Texture to use as the cursor image.</param>
        /// <param name="origin">The coordinates of the image that will be used for mouse position.</param>
        /// <param name="sourceRectangle">The part of the texture to use as the cursor.</param>
        public static MouseCursor FromTexture2D(
            Texture2D texture, Point origin, Rectangle? sourceRectangle = null)
        {
            if (texture.Format != SurfaceFormat.Rgba32 && texture.Format != SurfaceFormat.ColorSRgb)
                throw new ArgumentException(
                    $"Only {SurfaceFormat.Rgba32} or {SurfaceFormat.ColorSRgb} textures are accepted for mouse cursors.",
                    nameof(texture));

            var rect = sourceRectangle ?? texture.Bounds;
            var textureData = new Color[rect.Width * rect.Height];
            texture.GetData(textureData.AsSpan(), sourceRectangle);

            using (var image = Image.WrapMemory(textureData.AsMemory(), rect.Width, rect.Height))
                return FromImage(image, origin, sourceRectangle);
        }

        /// <summary>
        /// Creates a mouse cursor from the specified image.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the image.</typeparam>
        /// <param name="image">Image to use as the cursor image.</param>
        /// <param name="origin">The coordinates of the image that will be used for mouse position.</param>
        /// <param name="sourceRectangle">The part of the image to use as the cursor.</param>
        [CLSCompliant(false)]
        public static MouseCursor FromImage<TPixel>(
            Image<TPixel> image, Point origin, Rectangle? sourceRectangle = null)
            where TPixel : unmanaged, IPackedVector<TPixel>
        {
            var rect = sourceRectangle ?? new Rectangle(0, 0, image.Width, image.Height);

            Span<Color> imageData;
            if (image is Image<Color> rgbaImage)
            {
                // CreateRGBSurfaceFrom takes pitch which defines bytes per row so we
                // don't need to worry about indexing if the image is larger than the srcRect
                imageData = rgbaImage.GetPixelSpan();
            }
            else
            {
                var buffer = new Color[rect.Width * rect.Height];
                var pixels = image.GetPixelSpan();
                for (int y = 0; y < rect.Height; y++)
                {
                    var clippedPixelRow = pixels.Slice(rect.Y + y, rect.Width);
                    for (int x = 0; x < rect.Width; x++)
                        clippedPixelRow[rect.X + x].ToRgba32(ref buffer[x + y * rect.Height]);
                }
                imageData = buffer;
            }

            int stride = image.Width * image.PixelType.BitsPerPixel / 8; // bytes per row
            return PlatformFromImage(imageData, rect.Width, rect.Height, stride, origin);
        }

        static MouseCursor()
        {
            PlatformInitalize();
        }

        private MouseCursor(IntPtr handle)
        {
            Handle = handle;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            PlatformDispose();
            _disposed = true;
        }
    }
}
