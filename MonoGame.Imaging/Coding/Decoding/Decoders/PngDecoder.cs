using MonoGame.Imaging.Attributes.Coder;
using StbSharp;
using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class PngDecoder : StbDecoderBase, ICancellableCoderAttribute
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override bool TestFormat(
            ImagingConfig config, ReadContext context)
        {
            return stbi__png_test(context) != 0;
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return stbi__png_info(context, out readState);
        }

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
