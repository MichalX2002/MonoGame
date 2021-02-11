using System.IO;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageDecoder : StbImageDecoderBase<DecoderOptions>
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        public BmpImageDecoder(IImagingConfig config, Stream stream, DecoderOptions? decoderOptions) :
            base(config, stream, decoderOptions)
        {
        }

        protected override void Read()
        {
            var bmpInfo = StbSharp.ImageRead.Bmp.Load(Reader, ReadState, RecyclableArrayPool.Shared);
        }
    }
}
