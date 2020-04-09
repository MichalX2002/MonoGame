using System;
using System.IO;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coding.Encoding
{
    /// <summary>
    /// Represents a progress update for image encoding.
    /// </summary>
    public delegate void EncodeProgressCallback(
        ImageEncoderState encoderState,
        double percentage,
        Rectangle? rectangle);

    public abstract class ImageEncoderState : ImageCodecState
    {
        /// <summary>
        /// Gets the encoder that the state originates from.
        /// </summary>
        public IImageEncoder Encoder { get; }

        public IReadOnlyPixelRows CurrentImage { get; protected set; }

        public event EncodeProgressCallback Progress;

        public ImageEncoderState(
            ImagingConfig config,
            IImageEncoder encoder,
            Stream stream,
            bool leaveOpen) : base(config, stream, leaveOpen)
        {
            Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        protected void InvokeProgress(double percentage, Rectangle? rectangle)
        {
            Progress?.Invoke(this, percentage, rectangle);
        }
    }
}
