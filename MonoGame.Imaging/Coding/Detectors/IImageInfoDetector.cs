
namespace MonoGame.Imaging.Coding.Decoding
{
    /// <summary>
    /// Encapsulates detection of image formats and reading of image information.
    /// </summary>
    public interface IImageInfoDetector : IImageCoder
    {
        /// <summary>
        /// Tries to detect the format of an image from a stream.
        /// </summary>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The format that was detected.</returns>
        ImageFormat DetectFormat(ImagingConfig config, ImageReadStream stream);

        /// <summary>
        /// Tries to identify information about an image from a stream.
        /// </summary>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The information that was identified.</returns>
        ImageInfo Identify(ImagingConfig config, ImageReadStream stream);
    }
}
