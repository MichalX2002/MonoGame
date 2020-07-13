using System;
using MonoGame.Imaging.Coders.Detection;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats.Png
{
    public class PngImageFormatDetector : StbImageFormatDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override int HeaderSize => 8;

        protected override bool TestFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            return ImageRead.Png.Test(header);
        }
    }
}
