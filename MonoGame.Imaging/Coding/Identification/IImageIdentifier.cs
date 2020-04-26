using System;

namespace MonoGame.Imaging.Coding.Identification
{
    /// <summary>
    /// Encapsulates detection of image formats and reading of image information.
    /// </summary>
    public interface IImageIdentifier : IImageCodec
    {
        /// <summary>
        /// Gets the maximum size of the image header.
        /// </summary>
        int HeaderSize { get; }

        /// <summary>
        /// Tries to detect the format of an image from a data span.
        /// </summary>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="header">The data to inspect.</param>
        /// <returns>The format that was detected.</returns>
        ImageFormat DetectFormat(ImagingConfig config, ReadOnlySpan<byte> header);

        /// <summary>
        /// Tries to identify information about an image from a stream.
        /// </summary>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The information that was identified.</returns>
        ImageInfo Identify(ImagingConfig config, ImageReadStream stream);
    }
}
