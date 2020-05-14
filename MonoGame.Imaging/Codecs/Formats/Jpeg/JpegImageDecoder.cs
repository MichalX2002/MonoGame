using System.Threading.Tasks;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Codecs.Decoding;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Jpeg
{
    public class JpegImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Jpeg;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override async Task<bool> Read(
            StbImageDecoderState decoderState, ImageRead.ReadState readState)
        {
            await ImageRead.Jpeg.Load(
                decoderState.Reader, readState, RecyclableArrayPool.Default);

            return true;
        }
    }
}
