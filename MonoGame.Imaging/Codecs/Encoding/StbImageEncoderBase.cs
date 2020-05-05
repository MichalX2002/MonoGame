using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Encoding
{
    public abstract partial class StbImageEncoderBase : IImageEncoder
    {
        public abstract ImageFormat Format { get; }
        public virtual EncoderOptions DefaultOptions => EncoderOptions.Default;

        CodecOptions IImageCodec.DefaultOptions => DefaultOptions;

        protected abstract Task Write(
            StbImageEncoderState encoderState, ImageWrite.WriteState writeState);

        public ImageEncoderState CreateState(
            IImagingConfig imagingConfig,
            Stream stream, 
            bool leaveOpen, 
            CancellationToken cancellationToken = default)
        {
            return new StbImageEncoderState(
                this, imagingConfig, stream, leaveOpen, cancellationToken);
        }

        public Task Encode(
            ImageEncoderState encoderState,
            IReadOnlyPixelRows image)
        {
            if (encoderState == null) throw new ArgumentNullException(nameof(encoderState));
            if (image == null) throw new ArgumentNullException(nameof(image));

            var state = (StbImageEncoderState)encoderState;

            int components = 4; // TODO: change this so it's dynamic/controlled
            var provider = new PixelRowProvider(image, components, 8);

            var writeState = new ImageWrite.WriteState(
                state.Stream,
                state.CancellationToken,
                getPixelByteRow: provider.GetRow,
                getPixelFloatRow: provider.GetRow,
                state.ProgressCallback,
                image.Size.Width,
                image.Size.Height,
                components);

            return Write(state, writeState);
        }
    }
}
