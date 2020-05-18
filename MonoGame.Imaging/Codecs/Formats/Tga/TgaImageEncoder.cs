using System;
using MonoGame.Imaging.Attributes.Codec;
using MonoGame.Imaging.Codecs.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats
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

    namespace Tga
    {
        public class TgaImageEncoder : StbImageEncoderBase, ICancellableCodecAttribute
        {
            public override ImageFormat Format => ImageFormat.Tga;
            public override EncoderOptions DefaultOptions => TgaEncoderOptions.Default;

            protected override void Write(
                StbImageEncoderState encoderState,
                IReadOnlyPixelRows image,
                ImageWrite.WriteState writeState)
            {
                var options = encoderState.GetCodecOptions<TgaEncoderOptions>();
                
                ImageWrite.Tga.Write(writeState, options.UseRunLengthEncoding);
            }
        }
    }
}