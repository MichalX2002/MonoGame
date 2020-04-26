using System;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class TgaIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Tga;

        public override int HeaderSize => 18;

        protected override bool TestFormat(ImagingConfig config, ReadOnlySpan<byte> header)
        {
            return Tga.Test(header);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return Tga.Info(context, out readState);
        }
    }
}
