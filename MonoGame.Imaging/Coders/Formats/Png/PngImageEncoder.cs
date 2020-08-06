using System;
using System.IO.Compression;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats
{
    [Serializable]
    public class PngEncoderOptions : EncoderOptions
    {
        public static new PngEncoderOptions Default { get; } =
            new PngEncoderOptions(CompressionLevel.Fastest);

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
                if (encoderState == null) throw new ArgumentNullException(nameof(encoderState));
                if (image == null) throw new ArgumentNullException(nameof(image));
                if (writeState == null) throw new ArgumentNullException(nameof(writeState));

                var options = encoderState.GetCoderOptionsOrDefault<PngEncoderOptions>();
                
                // TODO: add forcedFilter option
                ImageWrite.Png.Write(writeState, options.CompressionLevel, null, RecyclableArrayPool.Shared);
            }
        }
    }
}