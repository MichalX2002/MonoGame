using System.IO;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageEncoder : StbImageEncoderBase<EncoderOptions>, ICancellableCoder
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        
        public BmpImageEncoder(IImagingConfig config, Stream stream, EncoderOptions? encoderOptions) : 
            base(config, stream, encoderOptions)
        {
        }
        
        protected override void Write(PixelRowProvider image)
        {
            // TODO: allow different bit depths

            StbSharp.ImageWrite.Bmp.Write(Writer, image);
        }
    }
}