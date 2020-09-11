using MonoGame.Imaging.Coders.Detection;
using StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Formats.Tga
{
    public class TgaImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override InfoResult GetInfo(IImagingConfig config, BinReader reader)
        {
            var tgaInfo = StbSharp.ImageRead.Tga.Info(reader, out var readState);

            return new InfoResult(readState);
        }
    }
}
