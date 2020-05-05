using System;
using MonoGame.Imaging.Codecs.Decoding;

namespace MonoGame.Imaging.Codecs.Detection
{
    public abstract class StbImageFormatDetectorBase  : IImageFormatDetector
    {
        public abstract ImageFormat Format { get; }
        public abstract int HeaderSize { get; }

        public CodecOptions DefaultOptions => CodecOptions.Default;

        protected abstract bool TestFormat(IImagingConfig config, ReadOnlySpan<byte> header);

        public ImageFormat? DetectFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            if (TestFormat(config, header))
                return Format;
            return null;
        }
    }
}
