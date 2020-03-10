using System;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Pixels;
using static StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coding.Encoding
{
    public abstract partial class StbImageEncoderBase : IImageEncoder
    {
        public abstract ImageFormat Format { get; }
        public abstract EncoderOptions DefaultOptions { get; }

        private static WriteState CreateWriteState(
            IReadOnlyPixelRows image,
            ImageStbEncoderState encoderState,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {
            var progressCallback = onProgress == null
                ? (WriteProgressCallback)null
                : (progress) => onProgress.Invoke(encoderState, progress, null);

            int components = 4; // TODO: change this so it's dynamic/controlled
            var provider = new RowsPixelProvider(image, components);

            var state = new WriteState(
                readBytePixels: provider.Fill,
                readFloatPixels: provider.Fill,
                progressCallback,
                image.Size.Width,
                image.Size.Height,
                components,
                encoderState.Stream,
                cancellationToken ?? CancellationToken.None,
                encoderState.ScratchBuffer);

            return state;
        }

        #region IImageEncoder

        public ImageEncoderState EncodeFirst(
            ImagingConfig imagingConfig,
            IReadOnlyPixelRows image,
            Stream stream, 
            EncoderOptions encoderOptions = null, 
            CancellationToken? cancellationToken = null, 
            EncodeProgressCallback onProgress = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();

            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            encoderOptions = ValidateEncoderOptions(encoderOptions);

            // TODO: do something about leaveOpen
            var encoderState = new ImageStbEncoderState(imagingConfig, this, stream, leaveOpen: true);
            var writeContext = CreateWriteState(image, encoderState, cancellationToken, onProgress);

            WriteFirst(encoderState.ImagingConfig, writeContext, image, encoderOptions);
            return encoderState;
        }

        public bool EncodeNext(
            ImageEncoderState encoderState,
            IReadOnlyPixelRows image, 
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null, 
            EncodeProgressCallback onProgress = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();

            if (encoderState == null) throw new ArgumentNullException(nameof(encoderState));
            if (image == null) throw new ArgumentNullException(nameof(image));
            encoderOptions = ValidateEncoderOptions(encoderOptions);

            var state = (ImageStbEncoderState)encoderState;
            var writeContext = CreateWriteState(image, state, cancellationToken, onProgress);

            return WriteNext(encoderState.ImagingConfig, writeContext, image, encoderOptions);
        }

        #endregion

        private EncoderOptions ValidateEncoderOptions(EncoderOptions encoderOptions)
        {
            if (encoderOptions == null)
                return DefaultOptions;

            EncoderOptions.AssertTypeEqual(DefaultOptions, encoderOptions, nameof(encoderOptions));
            return encoderOptions;
        }

        protected abstract bool WriteFirst(
            ImagingConfig imagingConfig,
            in WriteState state,
            IReadOnlyPixelRows image,
            EncoderOptions encoderOptions);

        protected virtual bool WriteNext(
            ImagingConfig imagingConfig,
            in WriteState state, 
            IReadOnlyPixelRows image,
            EncoderOptions encoderOptions)
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, imagingConfig);
            return false;
        }
    }
}
