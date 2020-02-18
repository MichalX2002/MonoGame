using StbSharp;
using static StbSharp.ImageRead;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class BmpDecoder : StbDecoderBase
    {
        public override ImageFormat Format => ImageFormat.Bmp;

        protected override unsafe IMemoryHolder ReadFirst(
            ImagingConfig imagingConfig,
            ReadContext context,
            ref ReadState state)
        {
            return Bmp.Load(context, ref state);
        }
    }
}
