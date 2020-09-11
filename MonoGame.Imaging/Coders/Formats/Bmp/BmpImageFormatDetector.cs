using System;
using MonoGame.Imaging.Coders.Detection;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageFormatDetector : StbImageFormatDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override int HeaderSize => StbSharp.ImageRead.Bmp.HeaderSize;

        protected override bool TestFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            return StbSharp.ImageRead.Bmp.Test(header);
        }
    }
}