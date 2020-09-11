using System;
using MonoGame.Imaging.Coders.Detection;

namespace MonoGame.Imaging.Coders.Formats.Tga
{
    public class TgaImageFormatDetector : StbImageFormatDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Tga;
        public override int HeaderSize => StbSharp.ImageRead.Tga.HeaderSize;

        protected override bool TestFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            return StbSharp.ImageRead.Tga.Test(header);
        }
    }
}
