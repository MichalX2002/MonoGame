using System;
using System.IO;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Attributes.Format;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coders.Formats.Gif
{
    public class GifImageEncoder : StbImageEncoderBase<EncoderOptions>, ICancellableCoder, IAnimatedFormatAttribute
    {
        public override ImageFormat Format => ImageFormat.Gif;

        public TimeSpan MinimumAnimationDelay => ((AnimatedImageFormat)Format).MinimumAnimationDelay;

        public GifImageEncoder(IImagingConfig config, Stream stream, EncoderOptions? encoderOptions) :
            base(config, stream, encoderOptions)
        {
        }

        public override bool CanEncodeImage(IReadOnlyPixelRows image)
        {
            AssertNotDisposed();

            return true;
        }

        protected override void Write(PixelRowProvider image)
        {
            // TODO: allow different bit depths

            throw new NotImplementedException();


            byte[] pixeldata = new byte[image.Width * image.Height * 4];
            for (int i = 0; i < image.Height; i++)
            {
                image.GetByteRow(i, pixeldata);
            }


        }
    }
}