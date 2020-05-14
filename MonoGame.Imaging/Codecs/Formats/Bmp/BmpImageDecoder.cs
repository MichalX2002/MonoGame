using System.Threading.Tasks;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Codecs.Decoding;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Bmp
{
    public class BmpImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override async Task<bool> Read(
            StbImageDecoderState decoderState, ImageRead.ReadState readState)
        {
            var bmpInfo = await ImageRead.Bmp.Load(
                decoderState.Reader, readState, RecyclableArrayPool.Default);

            return true;
        }
    }
}
