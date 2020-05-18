using System.IO;
using System.Threading;

namespace MonoGame.Imaging.Codecs.Decoding
{
    /// <summary>
    /// Encapsulates reading of image information.
    /// </summary>
    public interface IImageInfoDetector : IImageCodec
    {
        /// <summary>
        /// Tries to identify information about an image from a stream.
        /// </summary>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        /// <returns>The information that was identified.</returns>
        ImageInfo Identify(
            IImagingConfig config, Stream stream, CancellationToken cancellationToken = default);
    }
}
