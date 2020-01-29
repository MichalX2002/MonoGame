using System;
using System.IO;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coding.Encoding
{
    public class ImageEncoderState : ImageCoderState
    {
        /// <summary>
        /// Gets the encoder that the state originates from.
        /// </summary>
        public IImageEncoder Encoder { get; }

        public IPixelSource CurrentImage { get; set; }

        public ImageEncoderState(
            IImageEncoder encoder, 
            ImagingConfig imagingConfig,
            Stream stream,
            bool leaveOpen) : base(imagingConfig, stream, leaveOpen)
        {
            Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }
    }
}
