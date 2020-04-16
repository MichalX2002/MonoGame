using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class BmpImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override bool Read(StbImageDecoderState decoderState, ReadState readState)
        {
            return Bmp.Load(decoderState.Context, readState);
        }
    }
}
