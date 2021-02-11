using System.IO;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coders.Formats.Png
{
    public class PngImageEncoder : StbImageEncoderBase<PngEncoderOptions>, ICancellableCoder
    {
        public override ImageFormat Format => ImageFormat.Png;

        public PngImageEncoder(IImagingConfig config, Stream stream, PngEncoderOptions? encoderOptions) :
            base(config, stream, encoderOptions)
        {
        }
        
        protected override void Write(PixelRowProvider image)
        {
            // TODO: add forcedFilter option

            StbSharp.ImageWrite.Png.Write(
                Writer, image, EncoderOptions.CompressionLevel, null, null, RecyclableArrayPool.Shared);
        }
    }
}