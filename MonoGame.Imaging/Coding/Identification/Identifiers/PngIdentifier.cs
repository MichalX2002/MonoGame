using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class PngIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override bool TestFormat(ImagingConfig config, ReadContext context)
        {
            return Png.Test(context);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return Png.Info(context, out readState);
        }
    }
}
