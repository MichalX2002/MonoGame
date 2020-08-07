using System;
using System.IO;
using System.Threading;
using StbSharp;

namespace MonoGame.Imaging.Coders.Decoding
{
    public abstract partial class StbImageDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }

        public virtual DecoderOptions DefaultOptions => DecoderOptions.Default;
        CoderOptions IImageCoder.DefaultOptions => DefaultOptions;

        public ImageDecoderState CreateState(
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            return new StbImageDecoderState(this, config, stream, leaveOpen, cancellationToken);
        }

        public void Decode(ImageDecoderState decoderState)
        {
            if (decoderState == null)
                throw new ArgumentNullException(nameof(decoderState));

            var state = (StbImageDecoderState)decoderState;
            if (state.FrameIndex < 0)
                throw new InvalidOperationException("The decoder state is invalid.");

            var readState = state.CreateReadState();
            Read(state, readState);

            state.FrameIndex++;
        }

        protected abstract void Read(
            StbImageDecoderState decoderState, ImageRead.ReadState readState);
    }
}
