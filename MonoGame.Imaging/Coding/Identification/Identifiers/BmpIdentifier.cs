using System;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class BmpIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        public override int HeaderSize => 2;

        protected override bool TestFormat(ImagingConfig config, ReadOnlySpan<byte> header)
        {
            return Bmp.Test(header);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return Bmp.Info(context, out readState);
        }
    }
}
