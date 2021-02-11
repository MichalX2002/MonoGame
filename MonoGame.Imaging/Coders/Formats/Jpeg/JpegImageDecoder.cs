using System.IO;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;

namespace MonoGame.Imaging.Coders.Formats.Jpeg
{
    public class JpegImageDecoder : StbImageDecoderBase<DecoderOptions>
    {
        public override ImageFormat Format => ImageFormat.Jpeg;

        public JpegImageDecoder(IImagingConfig config, Stream stream, DecoderOptions? decoderOptions) :
            base(config, stream, decoderOptions)
        {
        }

        protected override void Read()
        {
            StbSharp.ImageRead.Jpeg.Load(Reader, ReadState, RecyclableArrayPool.Shared);
        }
    }
}
