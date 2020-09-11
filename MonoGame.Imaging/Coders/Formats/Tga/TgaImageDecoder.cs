using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;
using StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Formats.Tga
{
    public class TgaImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Tga;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override void Read(StbImageDecoderState decoderState, ReadState readState)
        {
            var tgaInfo = StbSharp.ImageRead.Tga.Load(
                decoderState.Reader, readState, RecyclableArrayPool.Shared);
        }
    }
}
