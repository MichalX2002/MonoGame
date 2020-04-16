using System;
using System.IO;

namespace MonoGame.Imaging.Coding
{
    public abstract class ImageCodecState : IImagingConfigProvider, IDisposable
    {
        private bool _leaveOpen;
        private CodecOptions _codecOptions;

        public bool IsDisposed { get; protected set; }

        public IImageCodec Codec { get; }
        public ImagingConfig ImagingConfig { get; }
        public Stream Stream { get; private set; }

        /// <summary>
        /// Gets the zero-based index of the most recently processed image.
        /// </summary>
        public int FrameIndex { get; protected set; }

        public CodecOptions CodecOptions
        {
            get => _codecOptions ?? Codec.DefaultOptions;
            set
            {
                if (value != null)
                    CodecOptions.AssertTypeEqual(Codec.DefaultOptions, value, nameof(value));
                _codecOptions = value;
            }
        }

        public ImageCodecState(IImageCodec codec, ImagingConfig config, Stream stream, bool leaveOpen)
        {
            Codec = codec ?? throw new ArgumentNullException(nameof(codec));
            ImagingConfig = config ?? throw new ArgumentNullException(nameof(config));
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveOpen = leaveOpen;
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            try
            {
                if (disposing)
                {
                    if (!_leaveOpen)
                        Stream?.Dispose();

                    Stream = null;
                }
            }
            finally
            {
                IsDisposed = true;
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
