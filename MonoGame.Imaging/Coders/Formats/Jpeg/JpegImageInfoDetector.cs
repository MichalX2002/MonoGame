using MonoGame.Imaging.Coders.Detection;
using StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Formats.Jpeg
{
    public class JpegImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Jpeg;

        protected override InfoResult GetInfo(IImagingConfig config, ImageBinReader reader)
        {
            StbSharp.ImageRead.Jpeg.Info(reader, out var readState);

            return new InfoResult(readState);
        }
    }
}
