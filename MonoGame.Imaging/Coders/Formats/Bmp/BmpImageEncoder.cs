using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Pixels;
using StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coders.Formats.Bmp
{
    public class BmpImageEncoder : StbImageEncoderBase, ICancellableCoderAttribute
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override EncoderOptions DefaultOptions => EncoderOptions.Default;

        protected override void Write(
            StbImageEncoderState encoderState,
            WriteState writeState,
            PixelRowProvider image)
        {
            // TODO: allow different bit depths

            StbSharp.ImageWrite.Bmp.Write(writeState, image);
        }
    }
}