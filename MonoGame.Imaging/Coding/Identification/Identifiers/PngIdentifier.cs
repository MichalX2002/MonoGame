using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Identification
{
    public class PngIdentifier : StbIdentifierBase
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
    }
}
