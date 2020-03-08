using MonoGame.Imaging.Attributes.Codec;
using StbSharp;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class PngDecoder : StbDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override bool ReadFirst(
            ImageStbDecoderState decoderState, ref ReadState readState)
        {
            throw new System.Exception("fix me");
            //return Png.Load(decoderState.ReadContext, ref readState);
        }
    }
}
