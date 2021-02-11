using System;
using System.IO.Compression;

namespace MonoGame.Imaging.Coders.Formats.Png
{
    [Serializable]
    public class PngEncoderOptions : EncoderOptions
    {
        public CompressionLevel CompressionLevel { get; }

        public PngEncoderOptions(CompressionLevel compression)
        {
            CompressionLevel = compression;
        }

        public PngEncoderOptions() : this(CompressionLevel.Fastest)
        {
        }
    }
}