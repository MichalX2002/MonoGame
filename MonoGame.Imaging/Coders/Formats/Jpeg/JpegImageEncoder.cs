using System.IO;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coders.Formats.Jpeg
{
    public class JpegImageEncoder : StbImageEncoderBase<JpegEncoderOptions>, ICancellableCoder
    {
        public override ImageFormat Format => ImageFormat.Jpeg;

        public JpegImageEncoder(IImagingConfig config, Stream stream, JpegEncoderOptions? encoderOptions) :
            base(config, stream, encoderOptions)
        {
        }

        protected override void Write(PixelRowProvider image)
        {
            bool useFloatPixels = image.Depth > 8;
            bool allowSubsample = EncoderOptions.Subsampling == JpegSubsampling.Allow;
            bool forceSubsample = EncoderOptions.Subsampling == JpegSubsampling.Force;

            StbSharp.ImageWrite.Jpeg.WriteCore(
                Writer, image, useFloatPixels, EncoderOptions.Quality, allowSubsample, forceSubsample);
        }
    }
}