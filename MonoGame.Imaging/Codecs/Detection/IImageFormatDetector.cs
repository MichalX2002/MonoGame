using System;

namespace MonoGame.Imaging.Codecs.Decoding
{
    /// <summary>
    /// Encapsulates detection of an image format.
    /// </summary>
    public interface IImageFormatDetector : IImageCodec
    {
        /// <summary>
        /// Gets the size of the image header.
        /// </summary>
        int HeaderSize { get; }

        /// <summary>
        /// Tries to detect the format of an image from a data span.
        /// </summary>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="header">The data to inspect.</param>
        /// <returns>The format that was detected.</returns>
        ImageFormat? DetectFormat(IImagingConfig config, ReadOnlySpan<byte> header);
    }
}
