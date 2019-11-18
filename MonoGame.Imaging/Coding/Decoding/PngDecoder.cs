using StbSharp;

namespace MonoGame.Imaging.Decoding
{
    public class PngDecoder : StbDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override bool TestFormat(StbImage.ReadContext context, ImagingConfig config)
        {
            return StbImage.stbi__png_test(context) != 0;
        }

        protected override bool GetInfo(
            StbImage.ReadContext context, ImagingConfig config, out int w, out int h, out int comp)
        {
            return StbImage.stbi__png_info(context, out w, out h, out comp);
        }

        protected override unsafe bool ReadFirst(
            StbImage.ReadContext context, ImagingConfig config,
            out void* result, ref StbImage.ReadState state)
        {
            result = StbImage.stbi__png_load(context, ref state);
            return result != null;
        }
    }
}
