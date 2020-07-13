using MonoGame.Imaging.Coders.Detection;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override InfoResult GetInfo(
            IImagingConfig config, ImageRead.BinReader reader)
        {
            var bmpInfo = ImageRead.Bmp.Info(reader, out var readState);

            return new InfoResult(readState);
        }
    }
}
