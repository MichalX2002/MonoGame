using System;
using System.IO;
using System.Threading;
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
        public event EncodeProgressCallback Progress;

        public IReadOnlyPixelRows CurrentImage { get; protected set; }
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets the encoder that the state originates from.
        /// </summary>
        public IImageEncoder Encoder => (IImageEncoder)Codec;

        public EncoderOptions EncoderOptions
        {
            get => (EncoderOptions)CodecOptions;
            set => CodecOptions = value;
        }

        public ImageEncoderState(
            IImageEncoder encoder,
            ImagingConfig config,
            Stream stream,
            bool leaveOpen) :
            base(encoder, config, stream, leaveOpen)
        {
        }

        protected void InvokeProgress(double percentage, Rectangle? rectangle)
        {
            Progress?.Invoke(this, percentage, rectangle);
        }
    }
}
