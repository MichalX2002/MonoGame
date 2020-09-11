using System;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;
using StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override void Read(StbImageDecoderState decoderState, ReadState readState)
        {
            if (decoderState == null)
                throw new ArgumentNullException(nameof(decoderState));

            var bmpInfo = StbSharp.ImageRead.Bmp.Load(
                decoderState.Reader, readState, RecyclableArrayPool.Shared);
        }
    }
}
