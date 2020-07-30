using System;
using System.Diagnostics;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Base class for objects that store pixels.
    /// </summary>
    public abstract partial class Image : IPixelMemory
    {
        public event Event<Image>? Disposing;

        #region Properties

        /// <summary>
        /// Gets whether the object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        public Size Size { get; }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public int Width => Size.Width;

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public int Height => Size.Height;

        /// <summary>
        /// Gets info about the pixel type of the image.
        /// </summary>
        public VectorType PixelType { get; }

        public abstract int ByteStride { get; }

        /// <summary>
        /// Gets the size of one pixel in bytes.
        /// </summary>
        public int ElementSize => PixelType.ElementSize;

        /// <summary>
        /// Gets the amount of pixels in the image.
        /// </summary>
        public int Length => Width * Height;

        #endregion

        static Image()
        {
            SetupReflection();
        }

        protected Image(VectorType pixelType, Size size)
        {
            PixelType = pixelType ?? throw new ArgumentNullException(nameof(pixelType));

            ArgumentGuard.AssertDimensionsGreaterThanZero(size, nameof(size), false);
            Size = size;
        }

        #region Create

        /// <summary>
        /// Creates an empty image with the given size and pixel type.
        /// </summary>
        public static Image Create(VectorType pixelType, Size size)
        {
            var createDelegate = GetCreateDelegate(pixelType);
            return createDelegate.Invoke(size, zeroFill: true);
        }

        /// <summary>
        /// Creates an empty image with the given size and pixel type.
        /// </summary>
        public static Image Create(VectorType pixelType, int width, int height)
        {
            return Create(pixelType, new Size(width, height));
        }

        /// <summary>
        /// Creates an image with the given size and pixel type
        /// without clearing the allocated memory.
        /// </summary>
        public static Image CreateUninitialized(VectorType pixelType, Size size)
        {
            var createDelegate = GetCreateDelegate(pixelType);
            return createDelegate.Invoke(size, zeroFill: false);
        }

        /// <summary>
        /// Creates an image with the given size and pixel type
        /// without clearing the allocated memory.
        /// </summary>
        public static Image CreateUninitialized(VectorType pixelType, int width, int height)
        {
            return CreateUninitialized(pixelType, new Size(width, height));
        }

        #endregion

        public abstract Span<byte> GetPixelByteRowSpan(int row);

        public abstract Span<byte> GetPixelByteSpan();

        public void GetPixelByteRow(int x, int y, Span<byte> destination)
        {
            var rowSpan = GetPixelByteRowSpan(y);
            rowSpan.Slice(x * PixelType.ElementSize).CopyTo(destination);
        }

        public void SetPixelByteRow(int x, int y, ReadOnlySpan<byte> data)
        {
            var rowSpan = GetPixelByteRowSpan(y);
            var slice = rowSpan.Slice(x * PixelType.ElementSize);
            data.CopyTo(slice);
        }

        public void SetPixelByteColumn(int x, int y, ReadOnlySpan<byte> data)
        {
            int rows = data.Length / PixelType.ElementSize;
            for (int srcRow = 0; srcRow < rows; srcRow++)
            {
                var dstSlice = GetPixelByteRowSpan(srcRow + y).Slice(x * PixelType.ElementSize);
                var srcSlice = data.Slice(srcRow * PixelType.ElementSize, PixelType.ElementSize);
                srcSlice.CopyTo(dstSlice);
            }
        }

        ReadOnlySpan<byte> IReadOnlyPixelBuffer.GetPixelByteRowSpan(int row)
        {
            return GetPixelByteRowSpan(row);
        }

        ReadOnlySpan<byte> IReadOnlyPixelMemory.GetPixelByteSpan()
        {
            return GetPixelByteSpan();
        }

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
