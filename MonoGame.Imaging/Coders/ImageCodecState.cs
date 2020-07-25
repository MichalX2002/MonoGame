using System;
using System.IO;
using System.Threading;

namespace MonoGame.Imaging.Coders
{
    public abstract class ImageCoderState : IImagingConfigurable, IDisposable
    {
        public IImageCoder Coder { get; }
        public IImagingConfig Config { get; }
        public Stream Stream { get; private set; }
        public bool LeaveOpen { get; }
        public CancellationToken CancellationToken { get; }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the zero-based index of the most recently processed image.
        /// </summary>
        public int FrameIndex { get; protected set; }

        public CoderOptions? CoderOptions { get; set; }

        public ImageCoderState(
            IImageCoder coder,
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken)
        {
            Coder = coder ?? throw new ArgumentNullException(nameof(coder));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Stream = stream ?? throw new ArgumentNullException(nameof(config));
            LeaveOpen = leaveOpen;
            CancellationToken = cancellationToken;
        }

        public TOptions GetCoderOptionsOrDefault<TOptions>()
            where TOptions : CoderOptions
        {
            if (CoderOptions != null)
            {
                if (Coder.DefaultOptions.IsAssignableFrom(CoderOptions))
                    return (TOptions)CoderOptions;
            }
            return (TOptions)Coder.DefaultOptions;
        }

        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (!LeaveOpen)
                        Stream.Dispose();
                }
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
