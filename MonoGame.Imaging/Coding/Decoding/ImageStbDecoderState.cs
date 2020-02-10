namespace MonoGame.Imaging.Coding.Decoding
{
    internal class ImageStbDecoderState : ImageDecoderState
    {
        public new Image CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int ImageIndex { get => base.ImageIndex; set => base.ImageIndex = value; }

        public ImageStbDecoderState(
            IImageDecoder decoder, 
            ImagingConfig imagingConfig, 
            ImageReadStream stream) : 
            base(decoder, imagingConfig, stream)
        {
        }
    }
}
