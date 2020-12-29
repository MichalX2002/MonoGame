using System;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Attributes.Format;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coders.Formats.Gif
{
    public class GifImageEncoder : StbImageEncoderBase, ICancellableCoderAttribute, IAnimatedFormatAttribute
    {
        public override ImageFormat Format => ImageFormat.Gif;
        public override EncoderOptions DefaultOptions => EncoderOptions.Default;

        public TimeSpan MinimumAnimationDelay => ((AnimatedImageFormat)Format).MinimumAnimationDelay;

        public override ImageEncoderState CreateState(
            IImagingConfig imagingConfig,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            return new StbGifImageEncoderState(this, imagingConfig, stream, leaveOpen, cancellationToken);
        }

        protected override void Write(
            StbImageEncoderState encoderState,
            WriteState writeState,
            PixelRowProvider image)
        {
            // TODO: allow different bit depths

            var state = (StbGifImageEncoderState)encoderState;


            byte[] pixeldata = new byte[image.Width * image.Height * 4];
            for (int i = 0; i < image.Height; i++)
            {
                image.GetByteRow(i, pixeldata);
            }


        }
    }

    public class StbGifImageEncoderState : StbImageEncoderState
    {
        public StbGifImageEncoderState(
            IImageEncoder encoder,
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken) :
            base(encoder, config, stream, leaveOpen, cancellationToken)
        {
        }

        protected override void Dispose(bool disposing)
        {


            base.Dispose(disposing);
        }
    }
}