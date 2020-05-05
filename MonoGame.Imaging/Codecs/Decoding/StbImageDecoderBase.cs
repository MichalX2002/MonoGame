using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Decoding
{
    public abstract partial class StbImageDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }
        
        public virtual DecoderOptions DefaultOptions => DecoderOptions.Default;
        CodecOptions IImageCodec.DefaultOptions => DefaultOptions;

        public ImageDecoderState CreateState(
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            return new StbImageDecoderState(this, config, stream, leaveOpen, cancellationToken);
        }

        public async Task<bool> Decode(ImageDecoderState decoderState)
        {
            var state = (StbImageDecoderState)decoderState;
            if (state.FrameIndex < 0)
                throw new InvalidOperationException("The decoder state is invalid.");

            var readState = state.CreateReadState();
            if (!await Read(state, readState))
                return false;

            state.FrameIndex++;
            return true;
        }

        protected abstract Task<bool> Read(
            StbImageDecoderState decoderState, ImageRead.ReadState readState);
    }
}
