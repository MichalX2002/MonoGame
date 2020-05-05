using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Codecs.Encoding
{
    /// <summary>
    /// Encapsulates encoding of images to a stream.
    /// </summary>
    public interface IImageEncoder : IImageCodec
    {
        /// <summary>
        /// Gets the default options for this encoder.
        /// </summary>
        new EncoderOptions DefaultOptions { get; }

        /// <summary>
        /// Creates a state that can be used to encode images.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="leaveOpen">Whether to leave <paramref name="stream"/> open after disposal.</param>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        /// <returns>The state used to encoding images.</returns>
        ImageEncoderState CreateState(
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Encodes an image to the encoder state stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the frame collection.</typeparam>
        /// <param name="encoderState">The shared state used to encode images.</param>
        /// <param name="image">The image to encode.</param>
        Task Encode(ImageEncoderState encoderState, IReadOnlyPixelRows image);
    }
}