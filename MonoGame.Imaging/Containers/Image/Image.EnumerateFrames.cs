using System;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Coders.Decoding;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region CreateDecoderEnumeratorAsync(Stream)

        public static ImageDecoderEnumerator CreateDecoderEnumerator(
            IImagingConfig imagingConfig,
            Stream stream,
            DecoderOptions? decoderOptions = null,
            CancellationToken cancellationToken = default)
        {
            if (imagingConfig == null)
                throw new ArgumentNullException(nameof(imagingConfig));

            var prefixStream = imagingConfig.CreateStreamWithHeaderPrefix(stream);
            Memory<byte> prefix = prefixStream.GetPrefix(cancellationToken);
            ImageFormat? format = DetectFormat(imagingConfig, prefix.Span);
            if (format == null)
            {
                if (prefix.Length == 0)
                {
                    throw new UnknownImageFormatException(
                        "No bytes were read from the stream.");
                }
                else
                {
                    throw new UnknownImageFormatException(
                        $"Failed to recognize any format from the first {prefix.Length} bytes.");
                }
            }

            IImageDecoder decoder = imagingConfig.CreateDecoder(prefixStream, format, decoderOptions);
            return new ImageDecoderEnumerator(decoder, cancellationToken);
        }

        public static ImageDecoderEnumerator CreateDecoderEnumerator(
            Stream stream,
            DecoderOptions? decoderOptions = null,
            CancellationToken cancellationToken = default)
        {
            return CreateDecoderEnumerator(ImagingConfig.Default, stream, decoderOptions, cancellationToken);
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
