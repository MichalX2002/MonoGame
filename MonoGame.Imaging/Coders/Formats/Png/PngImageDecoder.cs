using System.IO;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;
using StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Formats.Png
{
    public class PngImageDecoder : StbImageDecoderBase<DecoderOptions>
    {
        public override ImageFormat Format => ImageFormat.Png;
        
        public PngImageDecoder(IImagingConfig config, Stream stream, DecoderOptions? decoderOptions) :
            base(config, stream, decoderOptions)
        {
        }

        protected override void Read()
        {
            StbSharp.ImageRead.Png.Load(Reader, ReadState, ScanMode.Load, RecyclableArrayPool.Shared);
        }
    }
}
