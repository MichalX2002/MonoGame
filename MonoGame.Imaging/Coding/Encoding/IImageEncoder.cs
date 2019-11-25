using System.IO;
using System.Threading;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Coding.Encoding
{
    /// <summary>
    /// Represents a progress update for image encoding.
    /// </summary>
    /// <typeparam name="TPixel"></typeparam>
    /// <param name="frameIndex"></param>
    /// <param name="frames"></param>
    /// <param name="percentage"></param>
    public delegate void EncodeProgressCallback<TPixel>(
        int frameIndex, IReadOnlyPixelBuffer<TPixel> frame, double percentage)
        where TPixel : unmanaged, IPixel;

    /// <summary>
    /// Encapsulates encoding of images to a stream.
    /// </summary>
    public interface IImageEncoder : IImageCoder
    {
        /// <summary>
        /// Gets the default configuration for this encoder.
        /// </summary>
        EncoderConfig DefaultConfig { get; }

        /// <summary>
        /// Encodes the first image to a stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the frame collection.</typeparam>
        /// <param name="image">The image to encode.</param>
        /// <param name="stream">The stream to output to.</param>
        /// <param name="encoderConfig">The encoder configuration.</param>
        /// <param name="imagingConfig">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting encode progress.</param>
        void EncodeFirst<TPixel>(
            IReadOnlyPixelBuffer<TPixel> image,
            Stream stream,
            EncoderConfig encoderConfig,
            ImagingConfig imagingConfig,
            CancellationToken cancellationToken,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        /// <summary>
        /// Encodes an image to a stream after an initial first.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the frame collection.</typeparam>
        /// <param name="image">The image to encode.</param>
        /// <param name="stream">The stream to output to.</param>
        /// <param name="encoderConfig">The encoder configuration.</param>
        /// <param name="imagingConfig">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting encode progress.</param>
        void EncodeNext<TPixel>(
            IReadOnlyPixelBuffer<TPixel> image,
            Stream stream,
            EncoderConfig encoderConfig,
            ImagingConfig imagingConfig,
            CancellationToken cancellationToken,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;
    }
}