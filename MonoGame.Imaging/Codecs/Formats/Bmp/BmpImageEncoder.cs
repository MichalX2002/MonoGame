using MonoGame.Imaging.Attributes.Codec;
using MonoGame.Imaging.Codecs.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Bmp
{
    public class BmpImageEncoder : StbImageEncoderBase, ICancellableCodecAttribute
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override EncoderOptions DefaultOptions => EncoderOptions.Default;

        protected override void Write(
            StbImageEncoderState encoderState,
            IReadOnlyPixelRows image,
            ImageWrite.WriteState writeState)
        {
            // TODO: allow different bit depths

            ImageWrite.Bmp.Write(writeState);
        }
    }
}