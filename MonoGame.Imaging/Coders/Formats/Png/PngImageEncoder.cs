using System;
using System.IO.Compression;
using System.Threading.Tasks;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats
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
        public class PngImageEncoder : StbImageEncoderBase, ICancellableCoderAttribute
        {
            public override ImageFormat Format => ImageFormat.Png;
            public override EncoderOptions DefaultOptions => PngEncoderOptions.Default;

            protected override void Write(
                StbImageEncoderState encoderState,
                IReadOnlyPixelRows image,
                ImageWrite.WriteState writeState)
            {
                var options = encoderState.GetCoderOptions<PngEncoderOptions>();
                
                ImageWrite.Png.Write(writeState, options.CompressionLevel);
            }
        }
    }
}