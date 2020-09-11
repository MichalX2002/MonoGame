using MonoGame.Imaging.Coders.Detection;
using StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override InfoResult GetInfo(IImagingConfig config, BinReader reader)
        {
            var bmpInfo = StbSharp.ImageRead.Bmp.Info(reader, out var readState);

            return new InfoResult(readState);
        }
    }
}
