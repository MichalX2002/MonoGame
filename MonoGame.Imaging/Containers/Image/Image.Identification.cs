using System;
using System.IO;
using System.Threading;

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

        public static ImageInfo Identify(
            IImagingConfig config, Stream stream, CancellationToken cancellationToken = default)
        {
            using (var prefixedStream = config.CreateStreamWithHeaderPrefix(stream, true))
            {
                var prefix = prefixedStream.GetPrefix(cancellationToken);

                var format = DetectFormat(config, prefix.Span);
                if (format == null)
                    throw new UnknownImageFormatException();

                var infoDetector = config.GetInfoDetector(format);
                return infoDetector.Identify(config, prefixedStream, cancellationToken);
            }
        }

        public static ImageInfo Identify(Stream stream, CancellationToken cancellationToken = default)
        {
            return Identify(ImagingConfig.Default, stream, cancellationToken);
        }

        #endregion
    }
}
