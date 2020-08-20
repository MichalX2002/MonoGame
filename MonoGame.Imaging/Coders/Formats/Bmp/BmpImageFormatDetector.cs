using System;
using MonoGame.Imaging.Coders.Detection;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageFormatDetector : StbImageFormatDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override int HeaderSize => ImageRead.Bmp.HeaderSize;

        protected override bool TestFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            return ImageRead.Bmp.Test(header);
        }
    }
}
