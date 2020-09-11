using System;
using MonoGame.Imaging.Coders.Detection;

namespace MonoGame.Imaging.Coders.Formats.Jpeg
{
    public class JpegImageFormatDetector : StbImageFormatDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Jpeg;
        public override int HeaderSize => StbSharp.ImageRead.Jpeg.HeaderSize;

        protected override bool TestFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            return StbSharp.ImageRead.Jpeg.Test(header);
        }
    }
}
