using System;
using System.IO.Compression;
using StbSharp;

namespace MonoGame.Imaging.Encoding
{
    [Serializable]
    public class PngEncoderConfig : EncoderConfig
    {
        public static PngEncoderConfig Default { get; } = new PngEncoderConfig(CompressionLevel.Optimal);

        public CompressionLevel Compression { get; }

        public PngEncoderConfig(CompressionLevel compression)
        {
            Compression = compression;
        }
    }

    public class PngEncoder : StbEncoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override EncoderConfig DefaultConfig => PngEncoderConfig.Default;

        protected override bool WriteFirst<TPixel>(
            in StbImageWrite.WriteContext context, ReadOnlyImageFrame<TPixel> frame, 
            EncoderConfig encoderConfig, ImagingConfig imagingConfig,
            EncodeProgressDelegate<TPixel> onProgress = null)
        {
            var config = encoderConfig as PngEncoderConfig;
            return StbImageWrite.stbi_write_png_core(context, config.Compression);
        }
    }
}
