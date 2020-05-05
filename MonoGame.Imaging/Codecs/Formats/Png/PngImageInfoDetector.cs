using System.Threading.Tasks;
using MonoGame.Imaging.Codecs.Detection;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Png
{
    public class PngImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override async Task<InfoResult> GetInfo(
            IImagingConfig config, ImageRead.BinReader reader)
        {
            await ImageRead.Png.Info(reader, out var readState);
            return new InfoResult(readState);
        }
    }
}
