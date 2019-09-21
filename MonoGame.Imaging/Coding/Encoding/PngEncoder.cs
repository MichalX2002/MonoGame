using System;
using System.IO.Compression;
using StbSharp;

namespace MonoGame.Imaging.Encoding
{
    [Serializable]
    public class PngEncoderConfig : EncoderConfig
    {
        public static PngEncoderConfig Default { get; } = new PngEncoderConfig(CompressionLevel.Optimal);

        public CompressionLevel CompressionLevel { get; }

        public PngEncoderConfig(CompressionLevel compression) => CompressionLevel = compression;
    }

    public class PngEncoder : StbEncoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override EncoderConfig DefaultConfig => PngEncoderConfig.Default;

        protected override bool WriteFirst<TPixel>(
            in StbImageWrite.WriteContext context, ReadOnlyImageFrame<TPixel> frame, 
            EncoderConfig encoderConfig, ImagingConfig imagingConfig)
        {
            var config = encoderConfig as PngEncoderConfig;
            return StbImageWrite.stbi_write_png_core(context, config.CompressionLevel);
        }
    }
}
