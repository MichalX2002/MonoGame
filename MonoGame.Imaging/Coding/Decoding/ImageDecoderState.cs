using System.IO;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Coding.Decoding
{
    /// <summary>
    /// Represents a progress update for image decoding.
    /// </summary>
    public delegate void DecodeProgressCallback(
        ImageDecoderState decoderState,
        double percentage,
        Rectangle? rectangle);

    public abstract class ImageDecoderState : ImageCodecState
    {
        public event DecodeProgressCallback Progress;

        /// <summary>
        /// Gets the decoder that the state originates from.
        /// </summary>
        public IImageDecoder Decoder => (IImageDecoder)Codec;

        public Image CurrentImage { get; protected set; }

        public VectorTypeInfo PreferredPixelType { get; set; }

        public ImageDecoderState(
            IImageDecoder decoder,
            ImagingConfig config,
            Stream stream) :
            base(decoder, config, stream, leaveOpen: false)
        {
        }

        protected void InvokeProgress(double percentage, Rectangle? rectangle)
        {
            Progress?.Invoke(this, percentage, rectangle);
        }
    }
}
