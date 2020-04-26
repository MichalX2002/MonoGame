using MonoGame.Imaging.Attributes.Codec;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class PngImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override bool Read(StbImageDecoderState decoderState, ReadState readState)
        {
            return Png.Load(decoderState.Context, readState, ScanMode.Load);
        }
    }
}
