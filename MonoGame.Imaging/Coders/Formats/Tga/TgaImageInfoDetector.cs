using MonoGame.Imaging.Coders.Detection;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats.Tga
{
    public class TgaImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override InfoResult GetInfo(
            IImagingConfig config, ImageRead.BinReader reader)
        {
            var tgaInfo = ImageRead.Tga.Info(reader, out var readState);

            return new InfoResult(readState);
        }
    }
}
