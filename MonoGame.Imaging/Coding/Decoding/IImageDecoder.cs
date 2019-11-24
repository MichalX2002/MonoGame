using MonoGame.Framework;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Decoding
{
    /// <summary>
    /// Represents a progress update for image decoding.
    /// </summary>
    public delegate void DecodeProgressCallback<TPixel>(
        int frameIndex, Image<TPixel> frame, double progress, Rectangle? rectangle)
        where TPixel : unmanaged, IPixel;

    /// <summary>
    /// Encapsulates detection of image formats,
    /// identification of image information and 
    /// decoding of images.
    /// </summary>
    public interface IImageDecoder : IImageCoder
    {
        #region TryDetectFormat

        /// <summary>
        /// Tries to detect the format of an image from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="format">The format that was detected.</param>
        /// <returns><see langword="true"/> if the identification succeeded.</returns>
        bool TryDetectFormat(
            ImageReadStream stream, ImagingConfig config, out ImageFormat format);

        #endregion

        #region TryIdentify

        /// <summary>
        /// Tries to identify information about an image from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="info">The information that was identified.</param>
        /// <returns><see langword="true"/> if the identification succeeded.</returns>
        bool TryIdentify(
            ImageReadStream stream, ImagingConfig config, out ImageInfo info);

        #endregion

        #region DecodeFirst

        /// <summary>
        /// Decodes the first image of a stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type that the image will be decoded into.</typeparam>
        /// <param name="stream">The stream to read from.</param
        /// <param name="config">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        Image<TPixel> DecodeFirst<TPixel>(
            ImageReadStream stream,
            ImagingConfig config,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        #endregion

        #region DecodeNext

        /// <summary>
        /// Decodes the next image of a stream after an initial first.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type that the image will be decoded into.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        Image<TPixel> DecodeNext<TPixel>(
            ImageReadStream stream,
            ImagingConfig config,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        #endregion
    }
}
