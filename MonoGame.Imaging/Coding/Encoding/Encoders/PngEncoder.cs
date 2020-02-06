using System;
using System.IO.Compression;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Coding
{
    [Serializable]
    public class PngEncoderOptions : EncoderOptions
    {
        public static PngEncoderOptions Default { get; } = new PngEncoderOptions(CompressionLevel.Optimal);

        public CompressionLevel CompressionLevel { get; }

        public PngEncoderOptions(CompressionLevel compression)
        {
            CompressionLevel = compression;
        }
    }

    namespace Encoding
    {
        public class PngEncoder : StbEncoderBase, ICancellableCoderAttribute
        {
            public override ImageFormat Format => ImageFormat.Png;
            public override EncoderOptions DefaultOptions => PngEncoderOptions.Default;

            protected override bool WriteFirst(
                ImagingConfig imagingConfig,
                in StbImageWrite.WriteContext context,
                IReadOnlyPixelBuffer image,
                EncoderOptions encoderOptions)
            {
                var options = encoderOptions as PngEncoderOptions;
                return StbImageWrite.Png.WriteCore(context, options.CompressionLevel);
            }
        }
    }
}