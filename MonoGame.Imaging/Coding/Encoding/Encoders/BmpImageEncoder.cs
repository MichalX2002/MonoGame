using MonoGame.Imaging.Attributes.Codec;
using static StbSharp.ImageWrite;

namespace MonoGame.Imaging.Coding.Encoding
{
    public class BmpImageEncoder : StbImageEncoderBase, ICancellableCodecAttribute
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override EncoderOptions DefaultOptions => EncoderOptions.Default;

        protected override bool Write(
            StbImageEncoderState encoderState,
            in WriteState writeState)
        {
            return Bmp.WriteCore(writeState);
        }
    }
}