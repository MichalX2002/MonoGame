using System;
using System.Threading;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coders.Encoding
{
    /// <summary>
    /// Encapsulates encoding of images to a stream.
    /// </summary>
    public interface IImageEncoder : IImageCoder, IImagingConfigurable, IDisposable
    {
        /// <summary>
        /// Gets the options for this encoder.
        /// </summary>
        EncoderOptions EncoderOptions { get; }

        bool CanEncodeImage(IReadOnlyPixelRows image);

        /// <summary>
        /// Encodes an image to the underlying stream.
        /// </summary>
        /// <param name="image">The image to encode.</param>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        void Encode(IReadOnlyPixelRows image, CancellationToken cancellationToken = default);
    }
}