using StbSharp;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class BmpDecoder : StbDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override bool TestFormat(StbImage.ReadContext context, ImagingConfig config)
        {
            return StbImage.stbi__bmp_test(context) != 0;
        }

        protected override bool GetInfo(
            StbImage.ReadContext context, ImagingConfig config, out int w, out int h, out int comp)
        {
            return StbImage.stbi__bmp_info(context, out w, out h, out comp);
        }

        protected override unsafe bool ReadFirst(
            StbImage.ReadContext context, ImagingConfig config,
            out void* result, ref StbImage.ReadState state)
        {
            result = StbImage.stbi__bmp_load(context, ref state);
            return result != null;
        }
    }
}
