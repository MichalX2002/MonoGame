using System;
using System.IO.Compression;
using MonoGame.Imaging.Attributes.Codec;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Coding.Encoding
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

    public class PngEncoder : StbEncoderBase, ICancellableCodecAttribute
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override EncoderOptions DefaultOptions => PngEncoderOptions.Default;

        protected override bool WriteFirst(
            ImagingConfig imagingConfig,
            in ImageWrite.WriteContext context,
            IReadOnlyPixelRows image,
            EncoderOptions encoderOptions)
        {
            var options = encoderOptions as PngEncoderOptions;
            return ImageWrite.Png.WriteCore(context, options.CompressionLevel);
        }
    }
}