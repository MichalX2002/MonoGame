using MonoGame.Framework;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Coding.Decoding
{
    /// <summary>
    /// Represents a progress update for image decoding.
    /// </summary>
    public delegate void DecodeProgressCallback(
        ImageDecoderState decoderState,
        double percentage,
        Rectangle? rectangle);

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
        /// <param name="config">The imaging configuration.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="image">The decoded image.</param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        /// <returns>The state used to continue decoding of subsequent images.</returns>
        ImageDecoderState DecodeFirst<TPixel>(
            ImagingConfig config,
            ImageReadStream stream,
            out Image<TPixel> image,
            DecodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel;

        /// <summary>
        /// Decodes the next image of a stream using the state from the first decode call. 
        /// </summary>
        /// <typeparam name="TPixel">The pixel type that the image will be decoded into.</typeparam>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="decoderState">The state from the first decode call.</param>
        /// <param name="image">The decoded image.</param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        /// <returns>Whether an image was read.</returns>
        bool DecodeNext<TPixel>(
            ImagingConfig config,
            ImageDecoderState decoderState,
            out Image<TPixel> image,
            DecodeProgressCallback onProgress = null)
            where TPixel : unmanaged, IPixel;

        /// <summary>
        /// Finishes a decoding operation.
        /// </summary>
        /// <param name="decoderState">The state from the first decode call.</param>
        void FinishState(ImageDecoderState decoderState);
    }
}
