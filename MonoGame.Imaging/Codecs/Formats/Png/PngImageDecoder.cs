using MonoGame.Framework.Memory;
using MonoGame.Imaging.Codecs.Decoding;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Png
{
    public class PngImageDecoder : StbImageDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Png;
        public override DecoderOptions DefaultOptions => DecoderOptions.Default;

        protected override void Read(
            StbImageDecoderState decoderState, ImageRead.ReadState readState)
        {
            ImageRead.Png.Load(
                decoderState.Reader, readState, ImageRead.ScanMode.Load, RecyclableArrayPool.Default);
        }
    }
}
