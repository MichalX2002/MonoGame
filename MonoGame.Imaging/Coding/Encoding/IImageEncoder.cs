using System.IO;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Coding.Encoding
{
    /// <summary>
    /// Represents a progress update for image encoding.
    /// </summary>
    public delegate void EncodeProgressCallback<TPixel>(
        ImageEncoderState<TPixel> encoderState, 
        double percentage, 
        Rectangle? rectangle)
        where TPixel : unmanaged, IPixel;

    /// <summary>
    /// Encapsulates encoding of images to a stream.
    /// </summary>
    public interface IImageEncoder : IImageCoder
    {
        /// <summary>
        /// Gets the default options for this encoder.
        /// </summary>
        EncoderOptions DefaultOptions { get; }

        /// <summary>
        /// Encodes the first image to a stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the frame collection.</typeparam>
        /// <param name="image">The image to encode.</param>
        /// <param name="stream">The stream to output to.</param>
        /// <param name="encoderOptions">The encoder options.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting encode progress.</param>
        /// <returns>The state used to continue encoding of subsequent images.</returns>
        ImageEncoderState<TPixel> EncodeFirst<TPixel>(
            IReadOnlyPixelBuffer<TPixel> image,
            Stream stream,
            EncoderOptions encoderOptions,
            ImagingConfig config,
            CancellationToken cancellationToken,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        /// <summary>
        /// Encodes an image to a stream after an initial first.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the frame collection.</typeparam>
        /// <param name="image">The image to encode.</param>
        /// <param name="encoderState">The state from the first encode call.</param>
        /// <param name="encoderOptions">The encoder options.</param>
        /// <param name="config">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting encode progress.</param>
        bool EncodeNext<TPixel>(
            IReadOnlyPixelBuffer<TPixel> image,
            ImageEncoderState<TPixel> encoderState,
            EncoderOptions encoderOptions,
            ImagingConfig config,
            CancellationToken cancellationToken,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        /// <summary>
        /// Finishes an encoding operation.
        /// </summary>
        /// <param name="encoderState">The state from the first encode call.</param>
        void FinishState<TPixel>(ImageEncoderState<TPixel> encoderState)
            where TPixel : unmanaged, IPixel;
    }
}