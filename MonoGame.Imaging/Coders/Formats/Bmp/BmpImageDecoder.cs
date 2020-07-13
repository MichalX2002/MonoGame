using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override void Read(
            StbImageDecoderState decoderState, ImageRead.ReadState readState)
        {
            var bmpInfo = ImageRead.Bmp.Load(
                decoderState.Reader, readState, RecyclableArrayPool.Default);
        }
    }
}
