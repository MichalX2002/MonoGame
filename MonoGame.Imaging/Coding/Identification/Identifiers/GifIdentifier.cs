using System;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Identification
{
    public class GifIdentifier : StbIdentifierBase
    {
        public override ImageFormat Format => ImageFormat.Gif;

        public override int HeaderSize => 6;

        protected override bool TestFormat(ImagingConfig config, ReadOnlySpan<byte> header)
        {
            return Gif.Test(header);
        }

        protected override bool GetInfo(
            ImagingConfig config, ReadContext context, out ReadState readState)
        {
            return Gif.Info(context, out readState);
        }
    }
}
