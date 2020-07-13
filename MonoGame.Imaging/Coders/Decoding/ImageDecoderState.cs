using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging.Coders.Decoding
{
    /// <summary>
    /// Represents a progress update for image decoding.
    /// </summary>
    public delegate void DecodeProgressCallback(
        ImageDecoderState decoderState,
        double percentage,
        Rectangle? rectangle);

    public abstract class ImageDecoderState : ImageCoderState
    {
        public event DecodeProgressCallback? Progress;

        public Image? CurrentImage { get; protected set; }
        public VectorType? PreferredPixelType { get; set; }

        /// <summary>
        /// Gets the decoder that the state originates from.
        /// </summary>
        public IImageDecoder Decoder => (IImageDecoder)Coder;

        public bool HasProgressListener => Progress != null;

        public DecoderOptions? DecoderOptions
        {
            get => (DecoderOptions?)CoderOptions;
            set => CoderOptions = value;
        }

        public ImageDecoderState(
            IImageDecoder decoder,
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken) :
            base(decoder, config, stream, leaveOpen, cancellationToken)
        {
        }

        protected void InvokeProgress(double percentage, Rectangle? rectangle)
        {
            Progress?.Invoke(this, percentage, rectangle);
        }
    }
}
