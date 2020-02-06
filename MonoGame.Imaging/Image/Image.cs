using System;
using System.Diagnostics;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Base class for objects that store pixels.
    /// </summary>
    public abstract partial class Image : IPixelMemory
    {
        public event DatalessEvent<Image> Disposing;

        public abstract int ByteStride { get; }

        public bool IsPixelContiguous => Width * PixelType.ElementSize == ByteStride;

        #region Properties

        /// <summary>
        /// Gets whether the object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets info about the pixel type of the image.
        /// </summary>
        public PixelTypeInfo PixelType { get; }

        int IElementContainer.ElementSize => PixelType.ElementSize;
        int IElementContainer.Count => Width * Height;

        #endregion

        protected Image(int width, int height, PixelTypeInfo pixelInfo)
        {
            CommonArgumentGuard.AssertAboveZero(width, nameof(width));
            CommonArgumentGuard.AssertAboveZero(height, nameof(height));
            PixelType = pixelInfo ?? throw new ArgumentNullException(nameof(pixelInfo));

            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates an empty image using the
        /// <see cref="Image{TPixel}.Image(int, int)"/> constructor.
        /// </summary>
        public static Image<TPixel> Create<TPixel>(int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return new Image<TPixel>(width, height);
        }

        public abstract Span<byte> GetPixelByteRowSpan(int row);

        public abstract Span<byte> GetPixelByteSpan();

        ReadOnlySpan<byte> IReadOnlyPixelBuffer.GetPixelByteRowSpan(int row) => GetPixelByteRowSpan(row);

        ReadOnlySpan<byte> IReadOnlyPixelMemory.GetPixelByteSpan() => GetPixelByteSpan();

        #region IDisposable

        [DebuggerHidden]
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Image));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Disposing?.Invoke(this);

                IsDisposed = true;
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
