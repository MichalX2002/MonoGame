using System.IO;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coders.Formats.Tga
{
    public class TgaImageEncoder : StbImageEncoderBase<TgaEncoderOptions>, ICancellableCoder
    {
        public override ImageFormat Format => ImageFormat.Tga;

        public TgaImageEncoder(IImagingConfig config, Stream stream, TgaEncoderOptions? encoderOptions) :
            base(config, stream, encoderOptions)
        {
        }

        protected override void Write(PixelRowProvider image)
        {
            StbSharp.ImageWrite.Tga.Write(Writer, image, EncoderOptions.UseRunLengthEncoding);
        }
    }
}