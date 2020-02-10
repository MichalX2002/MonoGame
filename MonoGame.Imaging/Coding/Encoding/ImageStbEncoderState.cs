using System.IO;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coding.Encoding
{
    public class ImageStbEncoderState : ImageEncoderState
    {
        public new IReadOnlyPixelRows CurrentImage { get => base.CurrentImage; set => base.CurrentImage = value; }
        public new int ImageIndex { get => base.ImageIndex; set => base.ImageIndex = value; }

        public ImageStbEncoderState(
            ImagingConfig config, 
            IImageEncoder encoder, 
            Stream stream, 
            bool leaveOpen) :
            base(config, encoder, stream, leaveOpen)
        {
        }
    }
}
