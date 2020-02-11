using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Identification
{
    public class BmpIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override bool TestFormat(
            ImagingConfig config, ReadContext context)
        {
            return stbi__bmp_test(context) != 0;
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return stbi__bmp_info(context, out readState);
        }
    }
}
