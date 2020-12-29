using System;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coders.Formats
{
    [Serializable]
    public class TgaEncoderOptions : EncoderOptions
    {
        public static new TgaEncoderOptions Default { get; } = 
            new TgaEncoderOptions(useRunLengthEncoding: true);

        public static TgaEncoderOptions NoRLE { get; } =
            new TgaEncoderOptions(useRunLengthEncoding: false);

        public bool UseRunLengthEncoding { get; }

        public TgaEncoderOptions(bool useRunLengthEncoding)
        {
            UseRunLengthEncoding = useRunLengthEncoding;
        }
    }

    namespace Tga
    {
        public class TgaImageEncoder : StbImageEncoderBase, ICancellableCoderAttribute
        {
            public override ImageFormat Format => ImageFormat.Tga;
            public override EncoderOptions DefaultOptions => TgaEncoderOptions.Default;

            protected override void Write(
                StbImageEncoderState encoderState,
                WriteState writeState,
                PixelRowProvider image)
            {
                var options = encoderState.GetCoderOptionsOrDefault<TgaEncoderOptions>();
                
                StbSharp.ImageWrite.Tga.Write(writeState, image, options.UseRunLengthEncoding);
            }
        }
    }
}