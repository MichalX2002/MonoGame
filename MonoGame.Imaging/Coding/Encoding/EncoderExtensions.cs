using System.IO;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Encoding
{
    public static class EncoderExtensions
    {
        /// <summary>
        /// Encodes a collection of frames to a stream using the default encoder configuration.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type of the frame collection.</typeparam>
        /// <param name="frames">The collection of frames to encode.</param>
        /// <param name="stream">The stream to output to.</param>
        /// <param name="imagingConfig">The imaging configuration.</param>
        /// <param name="onProgress">Optional delegate for reporting encode progress.</param>
        public static void Encode<TPixel>(this IImageEncoder encoder,
            ReadOnlyFrameCollection<TPixel> frames, Stream stream, ImagingConfig imagingConfig,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            encoder.Encode(frames, stream, encoder.DefaultConfig, imagingConfig, onProgress);
        }
    }
}