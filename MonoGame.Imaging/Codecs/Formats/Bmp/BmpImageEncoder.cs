using System.Threading.Tasks;
using MonoGame.Imaging.Attributes.Codec;
using MonoGame.Imaging.Codecs.Encoding;
using StbSharp;

namespace MonoGame.Imaging.Codecs.Formats.Bmp
{
    public class BmpImageEncoder : StbImageEncoderBase, ICancellableCodecAttribute
    {
        public override ImageFormat Format => ImageFormat.Bmp;
        public override EncoderOptions DefaultOptions => EncoderOptions.Default;

        protected override Task Write(
            StbImageEncoderState encoderState,
            ImageWrite.WriteState writeState)
        {
            return ImageWrite.Bmp.Write(writeState);
        }
    }
}