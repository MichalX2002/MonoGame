using System;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coders.Formats
{
    public enum JpegSubsampling
    {
        Allow,
        Disallow,
        Force,
    }

    [Serializable]
    public class JpegEncoderOptions : EncoderOptions
    {
        public static new JpegEncoderOptions Default { get; } = new JpegEncoderOptions(90);

        public int Quality { get; }
        public JpegSubsampling Subsampling { get; }
        
        public JpegEncoderOptions(int quality, JpegSubsampling subsampling = JpegSubsampling.Allow)
        {
            switch (subsampling)
            {
                case JpegSubsampling.Allow:
                case JpegSubsampling.Disallow:
                case JpegSubsampling.Force:
                    Subsampling = subsampling;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(subsampling));
            }

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
                WriteState writeState)
            {
                if (encoderState == null) throw new ArgumentNullException(nameof(encoderState));
                if (image == null) throw new ArgumentNullException(nameof(image));
                if (writeState == null) throw new ArgumentNullException(nameof(writeState));

                var options = encoderState.GetCoderOptionsOrDefault<JpegEncoderOptions>();

                bool useFloatPixels = writeState.Depth > 8;
                bool allowSubsample = options.Subsampling == JpegSubsampling.Allow;
                bool forceSubsample = options.Subsampling == JpegSubsampling.Force;

                StbSharp.ImageWrite.Jpeg.WriteCore(
                    writeState, useFloatPixels, options.Quality, allowSubsample, forceSubsample);
            }
        }
    }
}