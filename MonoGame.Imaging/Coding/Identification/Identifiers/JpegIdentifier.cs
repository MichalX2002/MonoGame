using System;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class JpegIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Jpeg;

        public override int HeaderSize => 11;

        protected override bool TestFormat(ImagingConfig config, ReadOnlySpan<byte> header)
        {
            return Jpeg.Test(header);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return Jpeg.Info(context, out readState);
        }
    }
}
