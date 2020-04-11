using System;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Coding.Decoding;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region EnumerateFrames(Stream)

        public static ImageDecoderEnumerator EnumerateFrames(
            ImagingConfig config,
            Stream stream,
            out ImageFormat format,
            CancellationToken cancellationToken = default)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            using (var imageStream = config.CreateReadStream(stream, cancellationToken))
            {
                if (!TryDetectFormat(config, imageStream, out format))
                    throw new UnknownImageFormatException();

                if (!TryGetDecoder(format, out var decoder))
                    throw new MissingDecoderException(format);

                return new ImageDecoderEnumerator(decoder, config, imageStream);
            }
        }

        public static ImageDecoderEnumerator EnumerateFrames(
            Stream stream,
            out ImageFormat format,
            CancellationToken cancellationToken = default)
        {
            return EnumerateFrames(ImagingConfig.Default, stream, out format, cancellationToken);
        }

        #endregion

        /* TODO: fix this (UnmanagedMemoryStream?) :)

        #region LoadFrames(IReadOnlyMemory)

        public static ImageDecoderEnumerator<TPixel> LoadFrames<TPixel>(
            IReadOnlyMemory<byte> data, ImagingConfig config, int? frameLimit, out ImageFormat format,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (data.IsEmpty()) throw new ArgumentEmptyException(nameof(data));

            format = DetectFormat(data, config, cancellation);
            var decoder = GetDecoder(format);
            return decoder.Decode(data, config, frameLimit, cancellation, onProgress);
        }

        public static ImageDecoderEnumerator<TPixel> LoadFrames<TPixel>(
            IReadOnlyMemory<byte> data, int? frameLimit, out ImageFormat format,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(
                data, ImagingConfig.Default, frameLimit, 
                out format, cancellation, onProgress);
        }

        #endregion

        */
    }
}
