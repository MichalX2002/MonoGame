using System;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class PsdIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Psd;

        public override int HeaderSize => 5;

        protected override bool TestFormat(ImagingConfig config, ReadOnlySpan<byte> header)
        {
            return Psd.Test(header);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return Psd.Info(context, out readState);
        }
    }
}
