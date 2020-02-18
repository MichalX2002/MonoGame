using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class BmpIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override bool TestFormat(ImagingConfig config, ReadContext context)
        {
            return Bmp.Test(context);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return Bmp.Info(context, out readState);
        }
    }
}
