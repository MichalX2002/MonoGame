using MonoGame.Imaging.Attributes.Coder;
using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class PngDecoder : StbDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override unsafe bool ReadFirst(
            ImagingConfig imagingConfig,
            ReadContext context,
            out void* result,
            ref ReadState state)
        {
            result = stbi__png_load(context, ref state);
            return result != null;
        }
    }
}
