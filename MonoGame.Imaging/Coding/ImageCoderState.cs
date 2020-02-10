using System;
using System.IO;

namespace MonoGame.Imaging.Coding
{
    public abstract class ImageCoderState : IDisposable
    {
        private bool _leaveOpen;

        public bool IsDisposed { get; protected set; }

        public ImagingConfig Config { get; }
        public Stream Stream { get; }

        /// <summary>
        /// Gets the zero-based index of the most recently processed image.
        /// </summary>
        public int ImageIndex { get; protected set; } = -1;

        public ImageCoderState(ImagingConfig config, Stream stream, bool leaveOpen)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveOpen = leaveOpen;
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
