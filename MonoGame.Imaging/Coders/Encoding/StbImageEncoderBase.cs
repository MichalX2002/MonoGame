using System;
using System.IO;
using System.Threading;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Utilities;
using StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coders.Encoding
{
    public abstract class StbImageEncoderBase<TOptions> : IImageEncoder, IProgressReportingCoder<IImageEncoder>
        where TOptions : EncoderOptions, new()
    {
        private readonly object _progressMutex = new object();
        private ImagingProgressCallback<IImageEncoder>? _progress;
        private WriteProgressCallback? _writeProgress;

        public event ImagingProgressCallback<IImageEncoder>? Progress
        {
            add
            {
                if (value == null)
                    return;

                lock (_progressMutex)
                {
                    _progress += value;
                    if (_writeProgress == null)
                    {
                        _writeProgress = (p, r) => _progress?.Invoke(this, p, r?.ToMGRect());
                        Writer.Progress += _writeProgress;
                    }
                }
            }
            remove
            {
                if (value == null)
                    return;

                lock (_progressMutex)
                {
                    _progress -= value;
                    if (_progress == null && _writeProgress != null)
                    {
                        Writer.Progress -= _writeProgress;
                        _writeProgress = null;
                    }
                }
            }
        }

        public IImagingConfig ImagingConfig { get; }
        public TOptions EncoderOptions { get; }
        public ImageBinWriter Writer { get; }

        public bool IsDisposed { get; private set; }
        public int FrameIndex { get; private set; }

        EncoderOptions IImageEncoder.EncoderOptions => EncoderOptions;
        CoderOptions IImageCoder.CoderOptions => EncoderOptions;

        public virtual bool CanReportProgressRectangle => false;

        public abstract ImageFormat Format { get; }

        public StbImageEncoderBase(
            IImagingConfig config, Stream stream, TOptions? encoderOptions)
        {
            ImagingConfig = config ?? throw new ArgumentNullException(nameof(config));
            EncoderOptions = encoderOptions ?? new();

            byte[] buffer = RecyclableMemoryManager.Default.GetBlock();
            Writer = new ImageBinWriter(stream, buffer);
        }

        protected abstract void Write(PixelRowProvider image);

        public virtual bool CanEncodeImage(IReadOnlyPixelRows image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            AssertNotDisposed();

            return FrameIndex == 0;
        }

        public void Encode(
            IReadOnlyPixelRows image,
            CancellationToken cancellationToken = default)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            AssertCanEncodeImage(image);

            var pixelInfo = image.PixelType.ComponentInfo;

            // TODO: change components to dynamic/controlled (maybe in encoder options)
            bool imageHasAlpha = pixelInfo.HasComponentType(VectorComponentChannel.Alpha);
            bool encoderSupportsAlpha = true; // TODO: implement (encoderState.Encoder.SupportedComponents HasAttribute();)
            int colors = 3; // TODO: also implement grayscale
            int alpha = (imageHasAlpha && encoderSupportsAlpha) ? 1 : 0;
            int components = colors + alpha;

            int imageMaxDepth = pixelInfo.MaxBitDepth;
            int encoderMinDepth = 8; // TODO: implement
            int encoderMaxDepth = 8; // TODO: implement
            int variableDepth = Math.Max(encoderMinDepth, imageMaxDepth);
            int depth = Math.Min(encoderMaxDepth, variableDepth);

            var provider = new PixelRowProvider(image, components, depth, cancellationToken);
            Write(provider);

            FrameIndex++;
        }

        protected void AssertCanEncodeImage(IReadOnlyPixelRows image)
        {
            if (!CanEncodeImage(image))
                throw new ImagingException("Image may not be written in the current state of the encoder.");
        }

        protected void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    byte[]? buffer = Writer.Buffer;
                    Writer.Dispose();
                    RecyclableMemoryManager.Default.ReturnBlock(buffer);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~StbImageEncoderBase()
        {
            Dispose(disposing: false);
        }
    }
}
