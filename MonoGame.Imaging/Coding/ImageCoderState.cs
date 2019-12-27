using System;
using System.IO;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Coding
{
    public abstract class ImageCoderState<TPixel> : IDisposable
        where TPixel : unmanaged, IPixel
    {
        private bool _leaveOpen;

        #region Auto Properties

        public bool IsDisposed { get; protected set; }

        public Stream Stream { get; }

        /// <summary>
        /// Gets or sets the zero-based index of the current image.
        /// </summary>
        public int ImageIndex { get; set; }

        #endregion

        public ImageCoderState(Stream stream, bool leaveOpen)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveOpen = leaveOpen;

            ImageIndex = -1;
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (!_leaveOpen)
                            Stream?.Dispose();
                    }
                }
                finally
                {
                    IsDisposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageCoderState()
        {
            Dispose(false);
        }

        #endregion
    }
}
