using System;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Decoding
{
    /// <summary>
    /// Represents a progress update for image decoding.
    /// </summary>
    public delegate void DecodeProgressCallback<TPixel>(
        int frameIndex, FrameCollection<TPixel> frames, double progress, Rectangle? rectangle)
        where TPixel : unmanaged, IPixel;

    /// <summary>
    /// Encapsulates detection of image formats,
    /// identification of image information and 
    /// decoding of images from a stream or memory.
    /// </summary>
    public interface IImageDecoder : IImageCoder
    {
        #region TryDetectFormat

        /// <summary>
        /// Tries to detect the format of an image from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="format">The format that was detected.</param>
        /// <returns><see langword="true"/> if the identification succeeded.</returns>
        bool TryDetectFormat(
            ImageReadStream stream, ImagingConfig config, out ImageFormat format);

        /// <summary>
        /// Tries to detect the format of an image from memory.
        /// </summary>
        /// <param name="data">The memory to read from.</param>
        /// <param name="format">The format that was detected.</param>
        /// <returns><see langword="true"/> if the identification succeeded.</returns>
        bool TryDetectFormat(
            ReadOnlySpan<byte> data, ImagingConfig config,
            CancellationToken cancellation, out ImageFormat format);

        #endregion

        #region TryIdentify

        /// <summary>
        /// Tries to identify information about an image from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="info">The information that was identified.</param>
        /// <returns><see langword="true"/> if the identification succeeded.</returns>
        bool TryIdentify(
            ImageReadStream stream, ImagingConfig config, out ImageInfo info);

        /// <summary>
        /// Tries to identify information about an image from memory.
        /// </summary>
        /// <param name="data">The memory to read from.</param>
        /// <param name="info">The information that was identified.</param>
        /// <returns><see langword="true"/> if the identification succeeded.</returns>
        bool TryIdentify(
            ReadOnlySpan<byte> data, ImagingConfig config,
            CancellationToken cancellation, out ImageInfo info);

        #endregion

        #region Decode

        // TODO: FIXME: properly handle ImageCollection type

        /// <summary>
        /// Decodes a stream into a collection of images.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="amountLimit">
        /// Optional limit for the amount of images that can be decoded,
        /// the default being <see cref="int.MaxValue"/>.
        /// </param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        /// <returns>The collection of decoded images.</returns>
        ImageCollection<TPixel, ImageFrame<TPixel>> Decode<TPixel>(
            ImageReadStream stream,
            ImagingConfig config,
            int? amountLimit,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        /// <summary>
        /// Decodes memory into a collection of images.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="data">The memory to read from.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="amountLimit">
        /// Optional limit for the amount of images that can be decoded,
        /// the default being <see cref="int.MaxValue"/>.
        /// </param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        /// <returns>The collection of decoded images.</returns>
        ImageCollection<TPixel, ImageFrame<TPixel>> Decode<TPixel>(
            ReadOnlySpan<byte> data,
            ImagingConfig config,
            int? amountLimit,
            CancellationToken cancellation,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        #endregion
    }
}
