using System;
using MonoGame.Imaging.Coders.Detection;

namespace MonoGame.Imaging.Coders.Formats.Png
{
    public class PngImageFormatDetector : StbImageFormatDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override int HeaderSize => StbSharp.ImageRead.Png.HeaderSize;

        protected override bool TestFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            return StbSharp.ImageRead.Png.Test(header);
        }
    }
}
