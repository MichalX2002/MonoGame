using StbSharp;

namespace MonoGame.Imaging.Decoding
{
    public class BmpDecoder : StbDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override bool TestFormat(StbImage.ReadContext context)
        {
            return StbImage.stbi__bmp_test(context) != 0;
        }

        protected override bool GetInfo(StbImage.ReadContext context, out int w, out int h, out int comp)
        {
            return StbImage.stbi__bmp_info(context, out w, out h, out comp);
        }

        protected override unsafe bool ReadFirst(
            StbImage.ReadContext context, out void* result, ref StbImage.LoadState state)
        {
            result = StbImage.stbi__bmp_load(context, ref state);
            return result != null;
        }
    }
}
