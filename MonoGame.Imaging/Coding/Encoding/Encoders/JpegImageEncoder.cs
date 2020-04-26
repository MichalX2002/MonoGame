using System;
using MonoGame.Imaging.Attributes.Codec;
using static StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coding.Encoding
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

    public class JpegImageEncoder : StbImageEncoderBase, ICancellableCodecAttribute
    {
        public override ImageFormat Format => ImageFormat.Jpeg;
        public override EncoderOptions DefaultOptions => JpegEncoderOptions.Default;

        protected override bool Write(
            StbImageEncoderState encoderState,
            in WriteState writeState)
        {
            var options = encoderState.GetCodecOptions<JpegEncoderOptions>();
            // TODO: utilize readFloatPixels
            return Jpeg.WriteCore(writeState, options.Quality, useFloatPixels: false);
        }
    }
}