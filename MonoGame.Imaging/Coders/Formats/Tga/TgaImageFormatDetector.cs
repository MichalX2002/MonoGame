using System;
using MonoGame.Imaging.Coders.Detection;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats.Tga
{
    public class TgaImageFormatDetector : StbImageFormatDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Tga;
        public override int HeaderSize => ImageRead.Tga.HeaderSize;

        protected override bool TestFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            return ImageRead.Tga.Test(header);
        }
    }
}
