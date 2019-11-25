
namespace MonoGame.Imaging.Coding.Decoding
{
    /// <summary>
    /// Encapsulates detection of image formats and reading of image information.
    /// </summary>
    public interface IImageIdentifier : IImageCoder
    {
        /// <summary>
        /// Tries to detect the format of an image from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="format">The format that was detected.</param>
        /// <returns><see langword="true"/> if the identification succeeded.</returns>
        bool TryDetectFormat(
            ImageReadStream stream, ImagingConfig config, out ImageFormat format);

        /// <summary>
        /// Tries to identify information about an image from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="info">The information that was identified.</param>
        /// <returns><see langword="true"/> if the identification succeeded.</returns>
        bool TryIdentify(
            ImageReadStream stream, ImagingConfig config, out ImageInfo info);
    }
}
