using MonoGame.Imaging.Codecs.Detection;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Jpeg
{
    public class JpegImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Jpeg;

        protected override InfoResult GetInfo(
            IImagingConfig config, ImageRead.BinReader reader)
        {
            ImageRead.Jpeg.Info(reader, out var readState);

            return new InfoResult(readState);
        }
    }
}
