// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Graphics;
using MonoGame.Imaging;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Processing;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Describes a mouse cursor.
    /// </summary>
    public partial class MouseCursor : IDisposable
    {
        #region System Cursors

        /// <summary>
        /// Gets the default arrow cursor.
        /// </summary>
        public static MouseCursor? Arrow { get; private set; }

        /// <summary>
        /// Gets the cursor that appears when the mouse is over text editing regions.
        /// </summary>
        public static MouseCursor? IBeam { get; private set; }

        /// <summary>
        /// Gets the waiting cursor that appears while the application/system is busy.
        /// </summary>
        public static MouseCursor? Wait { get; private set; }

        /// <summary>
        /// Gets the crosshair ("+") cursor.
        /// </summary>
        public static MouseCursor? Crosshair { get; private set; }

        /// <summary>
        /// Gets a cross between Arrow and Wait cursors.
        /// </summary>
        public static MouseCursor? WaitArrow { get; private set; }

        /// <summary>
        /// Gets the northwest/southeast ("\") cursor.
        /// </summary>
        public static MouseCursor? SizeNWSE { get; private set; }

        /// <summary>
        /// Gets the northeast/southwest ("/") cursor.
        /// </summary>
        public static MouseCursor? SizeNESW { get; private set; }

        /// <summary>
        /// Gets the horizontal west/east ("-") cursor.
        /// </summary>
        public static MouseCursor? SizeWE { get; private set; }

        /// <summary>
        /// Gets the vertical north/south ("|") cursor.
        /// </summary>
        public static MouseCursor? SizeNS { get; private set; }

        /// <summary>
        /// Gets the size all cursor which points in all directions.
        /// </summary>
        public static MouseCursor? SizeAll { get; private set; }

        /// <summary>
        /// Gets the cursor that points that something is invalid, usually a cross.
        /// </summary>
        public static MouseCursor? No { get; private set; }

        /// <summary>
        /// Gets the hand cursor, usually used for web links.
        /// </summary>
        public static MouseCursor? Hand { get; private set; }

        #endregion

        /// <summary>
        /// Gets whether this cursor is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the platform handle to the cursor.
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// Initializes system cursors.
        /// </summary>
        static MouseCursor()
        {
            PlatformInitalize();
        }

        private MouseCursor(IntPtr handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// Creates a mouse cursor from the specified texture.
        /// </summary>
        /// <param name="texture">Texture to use as the cursor image.</param>
        /// <param name="origin">A point in the pixels that is used as the cursor position.</param>
        /// <param name="sourceRectangle">Optional part of the texture to use as the cursor.</param>
        public static MouseCursor FromTexture2D(
            Texture2D texture, Point origin, Rectangle? sourceRectangle = null)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            using (var image = texture.ToImage<Color>(sourceRectangle))
                return FromPixels(image, origin);
        }

        /// <summary>
        /// Creates a mouse cursor from the specified pixels.
        /// </summary>
        /// <param name="pixels">Pixels to use as the cursor image.</param>
        /// <param name="origin">A point in the pixels that is used as the cursor position.</param>
        /// <param name="sourceRectangle">Optional part of the image to use as the cursor.</param>
        [CLSCompliant(false)]
        public static unsafe MouseCursor FromPixels(
            IReadOnlyPixelRows pixels, Point origin, Rectangle? sourceRectangle = null)
        {
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));

            Rectangle rect = sourceRectangle ?? pixels.GetBounds();
            if (!pixels.GetBounds().Contains(rect))
                throw new ArgumentOutOfRangeException(
                     nameof(sourceRectangle), "The source rectangle is outside the pixel buffer.");

            IReadOnlyPixelMemory<Color>? pixelBuffer = null;
            try
            {
                if (rect.Position == Point.Zero &&
                    pixels is IReadOnlyPixelMemory<Color> rgbaMemory &&
                    rgbaMemory.IsPixelContiguous())
                {
                    pixelBuffer = rgbaMemory;
                }
                else
                {
                    pixelBuffer = Image.LoadPixels<Color>(pixels.Project(x => x.Crop(rect)));
                }

                return PlatformFromPixels(pixelBuffer, rect.Width, rect.Height, origin);
            }
            finally
            {
                pixelBuffer?.Dispose();
            }
        }

        /// <summary>
        /// Releases the cursor handle.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                PlatformDispose();

                IsDisposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the cursor handle.
        /// </summary>
        ~MouseCursor()
        {
            Dispose(disposing: false);
        }
    }
}
