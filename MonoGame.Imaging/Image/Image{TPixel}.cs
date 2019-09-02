using System;
using System.Diagnostics;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image<TPixel> : IPixelMemory<TPixel>, IDisposable
        where TPixel : unmanaged, IPixel
    {
        public delegate void DisposeDelegate(Image<TPixel> sender, bool disposing);

        public event DisposeDelegate OnDisposing;
        public event DisposeDelegate OnDisposed;

        private Buffer _pixelBuffer;

        #region Public Properties

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the width of this image in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of this image in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the data stride (row width) including padding of the image in pixels.
        /// </summary>
        public unsafe int Stride => _pixelBuffer.Stride;

        #endregion

        #region Constructors

        internal Image(Buffer buffer, int width, int height)
        {
            if (buffer.IsEmpty) throw new ArgumentEmptyException(nameof(buffer));
            CommonArgumentGuard.AssertAboveZero(width, nameof(width));
            CommonArgumentGuard.AssertAboveZero(height, nameof(height));

            _pixelBuffer = buffer;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructs an empty image with the given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public Image(int width, int height)
        {
            CommonArgumentGuard.AssertAboveZero(width, nameof(width));
            CommonArgumentGuard.AssertAboveZero(height, nameof(height));
            unsafe
            {
                var ptr = new UnmanagedPointer<TPixel>(width * height, zeroFill: true);
                _pixelBuffer = new Buffer(ptr, width, leaveOpen: false);
                Width = width;
                Height = height;
            }
        }

        #endregion

        #region IDisposable

        [DebuggerHidden]
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Image<TPixel>));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                OnDisposing?.Invoke(this, disposing);

                _pixelBuffer.Dispose();

                IsDisposed = true;
                OnDisposed?.Invoke(this, disposing);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Image()
        {
            Dispose(false);
        }

        #endregion
    }
}