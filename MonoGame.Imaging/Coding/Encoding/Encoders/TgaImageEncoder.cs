using System;
using MonoGame.Imaging.Attributes.Codec;
using static StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coding.Encoding
{
    [Serializable]
    public class TgaEncoderOptions : EncoderOptions
    {
        public new static TgaEncoderOptions Default { get; } = new TgaEncoderOptions(true);

        public static TgaEncoderOptions NoRLE { get; } = new TgaEncoderOptions(false);

        public bool UseRunLengthEncoding { get; }

        public TgaEncoderOptions(bool useRunLengthEncoding)
        {
            UseRunLengthEncoding = useRunLengthEncoding;
        }
    }

    public class TgaImageEncoder : StbImageEncoderBase, ICancellableCodecAttribute
    {
        public override ImageFormat Format => ImageFormat.Tga;
        public override EncoderOptions DefaultOptions => TgaEncoderOptions.Default;

        protected override bool Write(
            StbImageEncoderState encoderState,
            in WriteState writeState)
        {
            var options = encoderState.GetCodecOptions<TgaEncoderOptions>();
            return Tga.WriteCore(writeState, options.UseRunLengthEncoding);
        }
    }
}