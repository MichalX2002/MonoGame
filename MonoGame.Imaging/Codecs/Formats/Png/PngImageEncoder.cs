using System;
using System.IO.Compression;
using System.Threading.Tasks;
using MonoGame.Imaging.Attributes.Codec;
using MonoGame.Imaging.Codecs.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats
{
    [Serializable]
    public class PngEncoderOptions : EncoderOptions
    {
        public new static PngEncoderOptions Default { get; } =
            new PngEncoderOptions(CompressionLevel.Optimal);

        public CompressionLevel CompressionLevel { get; }

        public PngEncoderOptions(CompressionLevel compression)
        {
            CompressionLevel = compression;
        }
    }

    namespace Png
    {
        public class PngImageEncoder : StbImageEncoderBase, ICancellableCodecAttribute
        {
            public override ImageFormat Format => ImageFormat.Png;
            public override EncoderOptions DefaultOptions => PngEncoderOptions.Default;

            protected override void Write(
                StbImageEncoderState encoderState,
                IReadOnlyPixelRows image,
                ImageWrite.WriteState writeState)
            {
                var options = encoderState.GetCodecOptions<PngEncoderOptions>();
                
                ImageWrite.Png.Write(writeState, options.CompressionLevel);
            }
        }
    }
}