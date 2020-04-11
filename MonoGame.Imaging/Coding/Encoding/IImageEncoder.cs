using System.IO;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coding.Encoding
{
    /// <summary>
    /// Encapsulates encoding of images to a stream.
    /// </summary>
    public interface IImageEncoder : IImageCodec
    {
        /// <summary>
        /// Gets the default options for this encoder.
        /// </summary>
        EncoderOptions DefaultOptions { get; }

        /// <summary>
        /// Creates a state that can be used to encode images.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <returns>The state used to encoding images.</returns>
        ImageEncoderState CreateState(
            ImagingConfig config,
            Stream stream);

        /// <summary>
        /// Encodes an image to the encoder state stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the frame collection.</typeparam>
        /// <param name="image">The image to encode.</param>
        /// <param name="encoderState">The state from the first encode call.</param>
        /// <param name="encoderOptions">The encoder options.</param>
        /// <param name="onProgress">Optional delegate for reporting encode progress.</param>
        bool Encode(ImageEncoderState encoderState, IReadOnlyPixelRows image);
    }
}