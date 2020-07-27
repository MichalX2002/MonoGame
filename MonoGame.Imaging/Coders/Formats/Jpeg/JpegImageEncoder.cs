using System;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats
{
    [Serializable]
    public class JpegEncoderOptions : EncoderOptions
    {
        public static new JpegEncoderOptions Default { get; } = new JpegEncoderOptions(90);

        public int Quality { get; }

        public JpegEncoderOptions(int quality)
        {
            Quality = quality;
        }
    }

    namespace Jpeg
    {
        public class JpegImageEncoder : StbImageEncoderBase, ICancellableCoderAttribute
        {
            public override ImageFormat Format => ImageFormat.Jpeg;
            public override EncoderOptions DefaultOptions => JpegEncoderOptions.Default;

            protected override void Write(
                StbImageEncoderState encoderState,
                IReadOnlyPixelRows image,
                ImageWrite.WriteState writeState)
            {
                if (encoderState == null) throw new ArgumentNullException(nameof(encoderState));
                if (image == null) throw new ArgumentNullException(nameof(image));
                if (writeState == null) throw new ArgumentNullException(nameof(writeState));

                var options = encoderState.GetCoderOptionsOrDefault<JpegEncoderOptions>();

                bool useFloatPixels = writeState.Depth > 8;

                ImageWrite.Jpeg.WriteCore(writeState, options.Quality, useFloatPixels);
            }
        }
    }
}