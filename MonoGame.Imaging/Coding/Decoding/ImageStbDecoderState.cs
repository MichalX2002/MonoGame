
namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageStbDecoderState : ImageDecoderState
    {
        public new Image CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int ImageIndex { get => base.ImageIndex; set => base.ImageIndex = value; }

        public ImageStbDecoderState(
            ImagingConfig imagingConfig,
            IImageDecoder decoder, 
            ImageReadStream stream) : 
            base(imagingConfig, decoder, stream)
        {
        }
    }
}
