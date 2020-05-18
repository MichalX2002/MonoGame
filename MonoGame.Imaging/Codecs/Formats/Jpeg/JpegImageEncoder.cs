using System;
using MonoGame.Imaging.Attributes.Codec;
using MonoGame.Imaging.Codecs.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats
{
    [Serializable]
    public class JpegEncoderOptions : EncoderOptions
    {
        public new static JpegEncoderOptions Default { get; } = new JpegEncoderOptions(90);

        public int Quality { get; }

        public JpegEncoderOptions(int quality)
        {
            Quality = quality;
        }
    }

    namespace Jpeg
    {
        public class JpegImageEncoder : StbImageEncoderBase, ICancellableCodecAttribute
        {
            public override ImageFormat Format => ImageFormat.Jpeg;
            public override EncoderOptions DefaultOptions => JpegEncoderOptions.Default;

            protected override void Write(
                StbImageEncoderState encoderState,
                IReadOnlyPixelRows image,
                ImageWrite.WriteState writeState)
            {
                var options = encoderState.GetCodecOptions<JpegEncoderOptions>();

                bool useFloatPixels = writeState.Depth > 8;

                // TODO: utilize readFloatPixels
                ImageWrite.Jpeg.WriteCore(writeState, options.Quality, useFloatPixels);
            }
        }
    }
}