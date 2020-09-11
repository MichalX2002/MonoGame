using System;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Coders.Decoding;
using StbSharp.ImageRead;

namespace MonoGame.Imaging.Coders.Formats.Png
{
    public class PngImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override void Read(StbImageDecoderState decoderState, ReadState readState)
        {
            if (decoderState == null)
                throw new ArgumentNullException(nameof(decoderState));

            StbSharp.ImageRead.Png.Load(
                decoderState.Reader, readState, 
                ScanMode.Load, RecyclableArrayPool.Shared);
        }
    }
}
