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

        public abstract int ByteStride { get; }

        public bool IsPixelContiguous => Width * PixelType.ElementSize == ByteStride;

        int IElementContainer.ElementSize => PixelType.ElementSize;
        int IElementContainer.Count => Width * Height;

        #endregion

        protected Image(int width, int height, PixelTypeInfo pixelType)
        {
            ArgumentGuard.AssertAboveZero(width, nameof(width));
            ArgumentGuard.AssertAboveZero(height, nameof(height));
            PixelType = pixelType ?? throw new ArgumentNullException(nameof(pixelType));
            Width = width;
            Height = height;
        }

        #region Create

        /// <summary>
        /// Creates an empty image.
        /// </summary>
        public static Image Create(int width, int height, PixelTypeInfo pixelType)
        {
        }

        /// <summary>
        /// Creates an empty image.
        /// </summary>
        public static Image Create(Size size, PixelTypeInfo pixelType)
        {
        }

        #endregion

        public abstract Span<byte> GetPixelByteRowSpan(int row);

        public abstract Span<byte> GetPixelByteSpan();

        public void GetPixelByteRow(int x, int y, Span<byte> destination)
        {
            var rowSpan = GetPixelByteRowSpan(y);
            rowSpan.Slice(x).CopyTo(destination);
        }

        public void SetPixelByteRow(int x, int y, ReadOnlySpan<byte> data)
        {
            var rowSpan = GetPixelByteRowSpan(y);
            data.CopyTo(rowSpan.Slice(x));
        }

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
