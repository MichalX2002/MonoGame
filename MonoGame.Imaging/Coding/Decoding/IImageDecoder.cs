using MonoGame.Framework;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Coding.Decoding
{
    /// <summary>
    /// Represents a progress update for image decoding.
    /// </summary>
    public delegate void DecodeProgressCallback<TPixel>(
        int frameIndex, Image<TPixel> frame, double progress, Rectangle? rectangle)
        where TPixel : unmanaged, IPixel;

    /// <summary>
    /// Encapsulates decoding of images.
    /// </summary>
    public interface IImageDecoder : IImageCoder
    {
        /// <summary>
        /// Decodes the first image of a stream and returns a
        /// state that can be used to decode following images.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type that the image will be decoded into.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        /// <returns>The state used to continue decoding of subsequent images.</returns>
        ImageDecoderState<TPixel> DecodeFirst<TPixel>(
            ImageReadStream stream,
            ImagingConfig config,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        /// <summary>
        /// Decodes the next image of a stream using the state from the first decode call. 
        /// </summary>
        /// <typeparam name="TPixel">The pixel type that the image will be decoded into.</typeparam>
        /// <param name="decoderState">The state from first decode call.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        /// <returns>The image decoded by this call. Can be <see langword="null"/>.</returns>
        Image<TPixel> DecodeNext<TPixel>(
            ImageDecoderState<TPixel> decoderState,
            ImagingConfig config,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;
    }
}
