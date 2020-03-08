using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class BmpDecoder : StbDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override bool ReadFirst(
            ImageStbDecoderState decoderState, ref ReadState readState)
        {
            return Bmp.Load(decoderState.Context, ref readState);
        }
    }
}
