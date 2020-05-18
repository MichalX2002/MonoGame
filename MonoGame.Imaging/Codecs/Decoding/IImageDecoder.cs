using System.IO;
using System.Threading;

namespace MonoGame.Imaging.Codecs.Decoding
{
    /// <summary>
    /// Encapsulates decoding of images.
    /// </summary>
    public interface IImageDecoder : IImageCodec
    {
        /// <summary>
        /// Gets the default options for this decoder.
        /// </summary>
        new DecoderOptions DefaultOptions { get; }

        /// <summary>
        /// Creates a state that can be used to decode images.
        /// </summary>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="leaveOpen">Whether to leave <paramref name="stream"/> open after disposal.</param>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        /// <returns>The state used to decode images.</returns>
        ImageDecoderState CreateState(
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Decodes an image from the decoder state stream. 
        /// </summary>
        /// <param name="decoderState">The shared state used to decode images.</param>
        void Decode(ImageDecoderState decoderState);
    }
}
