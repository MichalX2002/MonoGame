using System;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;
using StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Formats.Jpeg
{
    public class JpegImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Jpeg;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override void Read(StbImageDecoderState decoderState, ReadState readState)
        {
            if (decoderState == null)
                throw new ArgumentNullException(nameof(decoderState));

            StbSharp.ImageRead.Jpeg.Load(
                decoderState.Reader, readState, RecyclableArrayPool.Shared);
        }
    }
}
