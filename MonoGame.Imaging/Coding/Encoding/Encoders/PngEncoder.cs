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
}

namespace MonoGame.Imaging.Coding.Encoding
{
    public class PngEncoder : StbEncoderBase, ICancellableCoderAttribute
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override EncoderOptions DefaultOptions => PngEncoderOptions.Default;

        protected override bool WriteFirst<TPixel>(
            in StbImageWrite.WriteContext context, 
            IReadOnlyPixelBuffer<TPixel> image, 
            EncoderOptions encoderOptions,
            ImagingConfig config)
        {
            var options = encoderOptions as PngEncoderOptions;
            return StbImageWrite.Png.WriteCore(context, options.CompressionLevel);
        }
    }
}
