using System.IO;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;

namespace MonoGame.Imaging.Coders.Formats.Tga
{
    public class TgaImageDecoder : StbImageDecoderBase<DecoderOptions>
    {
        public override ImageFormat Format => ImageFormat.Tga;

        public TgaImageDecoder(IImagingConfig config, Stream stream, DecoderOptions? decoderOptions) : 
            base(config, stream, decoderOptions)
        {
        }

        protected override void Read()
        {
            var tgaInfo = StbSharp.ImageRead.Tga.Load(Reader, ReadState, RecyclableArrayPool.Shared);
        }
    }
}
