using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region DetectFormat

        public static ImageFormat? DetectFormat(IImagingConfig config, ReadOnlySpan<byte> header)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            foreach (var formatDetector in config.GetFormatDetectors())
            {
                var format = formatDetector.DetectFormat(config, header);
                if (format != null)
                    return format;
            }
            return null;
        }

        public static ImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return DetectFormat(ImagingConfig.Default, header);
        }

        #endregion

        #region Identify

        public static async Task<ImageInfo> IdentifyAsync(
            IImagingConfig config, Stream stream, CancellationToken cancellationToken = default)
        {
            using (var prefixedStream = config.CreateStreamWithHeaderPrefix(stream, true))
            {
                var prefix = await prefixedStream.GetPrefixAsync(cancellationToken);

                var format = DetectFormat(config, prefix.Span);
                if (format == null)
                    throw new UnknownImageFormatException();

                var infoDetector = config.GetInfoDetector(format);
                return await infoDetector.Identify(config, prefixedStream, cancellationToken);
            }
        }

        public static Task<ImageInfo> IdentifyAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            return IdentifyAsync(ImagingConfig.Default, stream, cancellationToken);
        }

        #endregion
    }
}
