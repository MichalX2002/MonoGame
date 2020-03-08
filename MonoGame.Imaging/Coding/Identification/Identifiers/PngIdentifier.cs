using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class PngIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        protected override bool TestFormat(ImagingConfig config, ReadContext context)
        {
            throw new System.Exception("fix me");
            //return Png.Test(context);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            throw new System.Exception("fix me");
            //return Png.Info(context, out readState);
        }
    }
}
