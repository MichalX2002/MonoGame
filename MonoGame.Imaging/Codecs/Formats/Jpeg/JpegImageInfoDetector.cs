using System.Threading.Tasks;
using MonoGame.Imaging.Codecs.Detection;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Jpeg
{
    public class JpegImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Jpeg;

        protected override async Task<InfoResult> GetInfo(
            IImagingConfig config, ImageRead.BinReader reader)
        {
            await ImageRead.Jpeg.Info(reader, out var readState);

            return new InfoResult(readState);
        }
    }
}
