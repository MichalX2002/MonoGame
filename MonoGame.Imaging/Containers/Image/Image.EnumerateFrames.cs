using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Imaging.Coders.Decoding;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region CreateDecoderEnumeratorAsync(Stream)

        public static ImageDecoderEnumerator CreateDecoderEnumerator(
            IImagingConfig config,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            using (var prefixStream = config.CreateStreamWithHeaderPrefix(stream, leaveOpen))
            {
                var prefix = prefixStream.GetPrefix();

                var format = DetectFormat(config, prefix.Span);
                if (format == null)
                    throw new UnknownImageFormatException();

                var decoder = config.GetDecoder(format);
                return new ImageDecoderEnumerator(
                    config, decoder, prefixStream, false, cancellationToken);
            }
        }

        public static ImageDecoderEnumerator CreateDecoderEnumerator(
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            return CreateDecoderEnumerator(
                ImagingConfig.Default, stream, leaveOpen, cancellationToken);
        }

        #endregion

        /* TODO: fix this (maybe go easy with UnmanagedMemoryStream?) :)

        #region LoadFrames(IReadOnlyMemory)

        public static ImageDecoderEnumerator<TPixel> LoadFrames<TPixel>(
            IReadOnlyMemory data, ImagingConfig config, int? frameLimit, out ImageFormat format,
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
            IReadOnlyMemory data, int? frameLimit, out ImageFormat format,
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
