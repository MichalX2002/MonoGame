using System;
using System.IO.Compression;
using StbSharp;

namespace MonoGame.Imaging.Coding
{
    [Serializable]
    public class PngEncoderConfig : EncoderConfig
    {
        public static PngEncoderConfig Default { get; } = new PngEncoderConfig(CompressionLevel.Optimal);

        public CompressionLevel CompressionLevel { get; }

        public PngEncoderConfig(CompressionLevel compression)
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
        public override EncoderConfig DefaultConfig => PngEncoderConfig.Default;

        protected override bool WriteFirst<TPixel>(
            in StbImageWrite.WriteContext context, ReadOnlyImageFrame<TPixel> frame, 
            EncoderConfig encoderConfig, ImagingConfig imagingConfig)
        {
            var config = encoderConfig as PngEncoderConfig;
            return StbImageWrite.Png.WriteCore(context, config.CompressionLevel);
        }
    }
}
