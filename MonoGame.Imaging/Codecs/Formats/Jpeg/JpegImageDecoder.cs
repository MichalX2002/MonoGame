using MonoGame.Framework.Memory;
using MonoGame.Imaging.Codecs.Decoding;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Jpeg
{
    public class JpegImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Jpeg;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override void Read(
            StbImageDecoderState decoderState, ImageRead.ReadState readState)
        {
            ImageRead.Jpeg.Load(
                decoderState.Reader, readState, RecyclableArrayPool.Default);
        }
    }
}
