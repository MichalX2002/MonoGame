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
    public interface IImageDecoder : IImageCodec
    {
        /// <summary>
        /// Decodes the first image of a stream and returns a
        /// state that can be used to decode following images.
        /// </summary>
        /// <param name="imagingConfig">The imaging configuration.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="pixelType">
        /// The pixel type of the resulting image.
        /// Can be <see langword="null"/> to load the image without any conversion.
        /// </param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        /// <returns>The state used to continue decoding of subsequent images.</returns>
        ImageDecoderState DecodeFirst(
            ImagingConfig imagingConfig,
            ImageReadStream stream,
            VectorTypeInfo pixelType = null,
            DecodeProgressCallback onProgress = null);

        /// <summary>
        /// Decodes the next image of a stream using the state from the first decode call. 
        /// </summary>
        /// <param name="decoderState">The state from the first decode call.</param>/// <param name="pixelType">
        /// The pixel type of the resulting image.
        /// Can be <see langword="null"/> to load the image without any conversion.
        /// </param>
        /// <param name="onProgress">Optional delegate for reporting decode progress.</param>
        void DecodeNext(
            ImageDecoderState decoderState,
            VectorTypeInfo pixelType = null,
            DecodeProgressCallback onProgress = null);
    }
}
