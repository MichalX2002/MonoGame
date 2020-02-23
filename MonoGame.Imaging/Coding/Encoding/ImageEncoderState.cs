using System;
using System.IO;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coding.Encoding
{
    public abstract class ImageEncoderState : ImageCodecState
    {
        /// <summary>
        /// Gets the encoder that the state originates from.
        /// </summary>
        public IImageEncoder Encoder { get; }

        public IReadOnlyPixelRows CurrentImage { get; protected set; }

        public ImageEncoderState(
            ImagingConfig config,
            IImageEncoder encoder, 
            Stream stream,
            bool leaveOpen) : base(config, stream, leaveOpen)
        {
            Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }
    }
}
