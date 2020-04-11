using MonoGame.Imaging.Attributes.Codec;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class PngImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override bool Read(StbImageDecoderState decoderState, ref ReadState readState)
        {
            return Png.Load(decoderState.Context, ref readState);
        }
    }
}
