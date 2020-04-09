using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class BmpImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override bool ReadFirst(
            StbImageDecoderState decoderState, ref ReadState readState)
        {
            return Bmp.Load(decoderState.Context, ref readState);
        }
    }
}
