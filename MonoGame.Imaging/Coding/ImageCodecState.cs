using System;
using System.IO;

namespace MonoGame.Imaging.Coding
{
    public abstract class ImageCodecState : IImagingConfigProvider, IDisposable
    {
        private bool _leaveOpen;

        public bool IsDisposed { get; protected set; }

        public ImagingConfig ImagingConfig { get; }
        public Stream Stream { get; }

        /// <summary>
        /// Gets the zero-based index of the most recently processed image.
        /// </summary>
        public int ImageIndex { get; protected set; } = -1;

        public ImageCodecState(ImagingConfig config, Stream stream, bool leaveOpen)
        {
            ImagingConfig = config ?? throw new ArgumentNullException(nameof(config));
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

        ~ImageCodecState()
        {
            Dispose(false);
        }

        #endregion
    }
}
