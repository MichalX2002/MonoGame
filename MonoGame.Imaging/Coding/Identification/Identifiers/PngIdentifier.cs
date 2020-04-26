using System;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class PngIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Png;

        public override int HeaderSize => 8;

        protected override bool TestFormat(ImagingConfig config, ReadOnlySpan<byte> header)
        {
            return Png.Test(header);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return Png.Info(context, out readState);
        }
    }
}
