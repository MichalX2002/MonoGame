using System;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract partial class StbImageDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }
        public virtual DecoderOptions DefaultOptions => DecoderOptions.Default;

        CodecOptions IImageCodec.DefaultOptions => DefaultOptions;

        protected abstract bool Read(StbImageDecoderState decoderState, ReadState readState);

        public ImageDecoderState CreateState(ImagingConfig config, ImageReadStream stream)
        {
            return new StbImageDecoderState(this, config, stream);
        }

        public bool Decode(ImageDecoderState decoderState)
        {
            var state = (StbImageDecoderState)decoderState;
            if (state.FrameIndex < 0)
                throw new InvalidOperationException("The decoder state is invalid.");

            var readState = state.CreateReadState();
            if (!Read(state, readState))
                return false;

            state.FrameIndex++;
            return true;
        }
    }
}
