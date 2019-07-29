using System;
using System.Diagnostics;
using MonoGame.Utilities.PackedVector;
using MonoGame.Utilities.Memory;

namespace MonoGame.Imaging
{
    public class ImageFrame<TPixel> : IDisposable
        where TPixel : unmanaged, IPixel
    {
        public delegate void DisposeDelegate(ImageFrame<TPixel> sender);

        public event DisposeDelegate OnDisposing;
        public event DisposeDelegate OnDisposed;

        private UnmanagedPointer<TPixel> _pixels;
        private ImageMetaData _metaData;

        #region Properties

        public bool IsDisposed { get; private set; }

        public ImageMetaData MetaData
        {
            get
            {
                AssertNotDisposed();
                return _metaData;
            }
        }

        public int Width
        {
            get
            {
                AssertNotDisposed();
                return _metaData.Width;
            }
        }

        public int Height
        {
            get
            {
                AssertNotDisposed();
                return _metaData.Height;
            }
        }

        #endregion

        #region Constructors

        public ImageFrame(ImageMetaData metaData)
        {
            _metaData = metaData;
            _pixels = new UnmanagedPointer<TPixel>(metaData.Width * metaData.Height);
        }

        #endregion

        #region GetPixel Span Methods

        public Span<TPixel> GetPixelSpan()
        {
            AssertNotDisposed();
            return _pixels.Span;
        }

        public Span<TPixel> GetPixelSpan(int start) => GetPixelSpan().Slice(start);

        public Span<TPixel> GetPixelSpan(int start, int length) => GetPixelSpan().Slice(start, length);

        public Span<TPixel> GetPixelRowSpan(int rowIndex) => GetPixelSpan(rowIndex * MetaData.Width);

        #endregion

        #region IDisposable

        [DebuggerHidden]
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ImageFrameCollection<TPixel>));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                OnDisposing?.Invoke(this);

                _pixels?.Dispose();
                _pixels = null;
                _metaData = default;

                IsDisposed = true;
                OnDisposed?.Invoke(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageFrame()
        {
            Dispose(false);
        }

        #endregion
    }
}