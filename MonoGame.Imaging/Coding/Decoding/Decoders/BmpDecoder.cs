using StbSharp;
using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class BmpDecoder : StbDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override unsafe bool ReadFirst(
            ImagingConfig imagingConfig,
            ReadContext context,
            out IMemoryResult result,
            ref ReadState state)
        {
            result = stbi__bmp_load(context, ref state);
            return result != null;
        }
    }
}
