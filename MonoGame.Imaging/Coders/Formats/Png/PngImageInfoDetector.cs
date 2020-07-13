using System.Threading.Tasks;
using MonoGame.Imaging.Coders.Detection;
using StbSharp;

namespace MonoGame.Imaging.Coders.Formats.Png
{
    public class PngImageInfoDetector : StbImageInfoDetectorBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override InfoResult GetInfo(
            IImagingConfig config, ImageRead.BinReader reader)
        {
            ImageRead.Png.Info(reader, out var readState);
            
            return new InfoResult(readState);
        }
    }
}
