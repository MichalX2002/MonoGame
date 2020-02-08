// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.PackedVector;
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

        /// <summary>
        /// Gets the platform handle to the cursor.
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// Creates a mouse cursor from the specified texture.
        /// </summary>
        /// <param name="texture">Texture to use as the cursor image.</param>
        /// <param name="origin">A point in the pixels that is used as the cursor position.</param>
        /// <param name="sourceRectangle">Optional part of the texture to use as the cursor.</param>
        public static MouseCursor FromTexture2D(
            Texture2D texture, Point origin, Rectangle? sourceRectangle = null)
        {
            if (texture.Format != SurfaceFormat.Rgba32 && texture.Format != SurfaceFormat.Rgba32SRgb)
                throw new ArgumentException(
                    $"Only {SurfaceFormat.Rgba32} and {SurfaceFormat.Rgba32SRgb} textures are accepted for mouse cursors.",
                    nameof(texture));

            var rect = sourceRectangle ?? texture.Bounds;

            using (var image = Image<Color>.Create(rect.Size))
            {
                texture.GetData(image.GetPixelSpan(), rect);
                return FromPixels(image, origin);
            }
        }

        /// <summary>
        /// Creates a mouse cursor from the specified pixels.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the buffer.</typeparam>
        /// <param name="pixels">Pixels to use as the cursor image.</param>
        /// <param name="origin">A point in the pixels that is used as the cursor position.</param>
        /// <param name="sourceRectangle">Optional part of the image to use as the cursor.</param>
        [CLSCompliant(false)]
        public static unsafe MouseCursor FromPixels<TPixel>(
            IReadOnlyPixelRows<TPixel> pixels, Point origin, Rectangle? sourceRectangle = null)
            where TPixel : unmanaged, IPixel
        {
            Rectangle rect = sourceRectangle ?? pixels.GetBounds();
            if (!pixels.GetBounds().Contains(rect))
                throw new ArgumentOutOfRangeException(
                    "The source rectangle is outside the pixel buffer.", nameof(sourceRectangle));

            IReadOnlyPixelMemory<Color> pixelBuffer = null;
            try
            {
                ReadOnlySpan<Color> pixelSpan;
                int stride;

                if (rect.Position == Point.Zero && pixels is IReadOnlyPixelMemory<Color> rgbaMemory)
                {
                    // PlatformFromPixels takes stride so we don't need to worry
                    // about a source rect whose width differs from the buffer's stride.
                    pixelSpan = rgbaMemory.GetPixelSpan();
                    stride = rgbaMemory.ByteStride;
                }
                else
                {
                    pixelBuffer = Image.LoadPixels<TPixel, Color>(pixels.Project(x => x.Crop(rect)));
                    pixelSpan = pixelBuffer.GetPixelSpan();
                    stride = pixelBuffer.ByteStride;
                }

                return PlatformFromPixels(pixelSpan, rect.Width, rect.Height, stride, origin);
            }
            finally
            {
                pixelBuffer?.Dispose();
            }
        }

        static MouseCursor()
        {
            PlatformInitalize();
        }

        private MouseCursor(IntPtr handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// Releases the cursor handle.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                PlatformDispose();
                _disposed = true;
            }
        }
    }
}
