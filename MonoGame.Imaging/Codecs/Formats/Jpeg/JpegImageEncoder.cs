using System;
using System.Threading.Tasks;
using MonoGame.Imaging.Attributes.Codec;
using MonoGame.Imaging.Codecs.Encoding;
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

            protected override Task Write(
                StbImageEncoderState encoderState,
                ImageWrite.WriteState writeState)
            {
                var options = encoderState.GetCodecOptions<JpegEncoderOptions>();
                // TODO: utilize readFloatPixels
                return ImageWrite.Jpeg.WriteCore(writeState, options.Quality, useFloatPixels: false);
            }
        }
    }
}