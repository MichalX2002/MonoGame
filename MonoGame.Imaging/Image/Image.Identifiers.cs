using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Coding.Identification;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        // TODO: move these to ImagingConfig

        private static List<IImageIdentifier> _infoDetectors;
        private static ConcurrentDictionary<ImageFormat, IImageIdentifier> _infoDetectorByFormat;

        private static List<IImageIdentifier> GetInfoDetectors()
        {
            if (_infoDetectors == null)
            {
                _infoDetectors = new List<IImageIdentifier>();
                _infoDetectors.Add(new PngIdentifier());
                _infoDetectors.Add(new BmpIdentifier());
            }
            return _infoDetectors;
        }

        #region [Try]GetInfoDetector

        public static bool TryGetInfoDetector(ImageFormat format, out IImageIdentifier infoDetector)
        {
            if (_infoDetectorByFormat == null)
            {
                _infoDetectorByFormat = new ConcurrentDictionary<ImageFormat, IImageIdentifier>();

                _infoDetectorByFormat.TryAdd(GetInfoDetectors()[0].Format, GetInfoDetectors()[0]);
                _infoDetectorByFormat.TryAdd(GetInfoDetectors()[1].Format, GetInfoDetectors()[1]);
            }
            return _infoDetectorByFormat.TryGetValue(format, out infoDetector);
        }

        public static IImageIdentifier GetInfoDetector(ImageFormat format)
        {
            if (TryGetInfoDetector(format, out var infoDetector))
                return infoDetector;
            throw new MissingInfoDetectorException(format);
        }

        #endregion

        #region [Try]DetectFormat

        public static bool TryDetectFormat(
            ImagingConfig config, ImageReadStream stream, out ImageFormat format)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            foreach (var infoDetector in GetInfoDetectors())
            {
                format = infoDetector.DetectFormat(config, stream);
                if (format != null)
                    return true;
            }
            format = default;
            return false;
        }

        public static bool TryDetectFormat(
            ImagingConfig config,
            Stream stream,
            CancellationToken cancellation, 
            out ImageFormat format)
        {
            using (var readStream = config.CreateReadStream(stream, cancellation))
                return TryDetectFormat(config, readStream, out format);
        }

        public static ImageFormat DetectFormat(
            ImagingConfig config, ImageReadStream imageStream)
        {
            if (TryDetectFormat(config, imageStream, out var format))
                return format;
            throw new UnknownImageFormatException();
        }

        public static ImageFormat DetectFormat(
            ImagingConfig config, Stream stream, CancellationToken cancellation)
        {
            using (var readStream = config.CreateReadStream(stream, cancellation))
                return DetectFormat(config, readStream);
        }

        /*

        public static bool TryDetectFormat(
            IReadOnlyMemory<byte> data, ImagingConfig config,
            CancellationToken cancellation, out ImageFormat format)
        {
            if (!data.IsEmpty())
            {
                foreach (var infoDetector in GetInfoDetectors())
                {
                    format = infoDetector.DetectFormat(data, config, cancellation);
                    if (format != null)
                        return true;
                }
            }
            format = default;
            return false;
        }

        public static ImageFormat DetectFormat(
            IReadOnlyMemory<byte> data, ImagingConfig config, CancellationToken cancellation)
        {
            if (TryDetectFormat(data, config, cancellation, out var format))
                return format;
            throw new UnknownImageFormatException();
        }

        */

        #endregion

        #region [Try]Identify

        public static bool TryIdentify(
            ImagingConfig config,
            Stream stream,
            CancellationToken cancellation,
            out ImageInfo info)
        {
            using (var readStream = config.CreateReadStream(stream, cancellation))
            {
                if (TryDetectFormat(config, readStream, out var format))
                {
                    if (TryGetInfoDetector(format, out var detector))
                    {
                        info = detector.Identify(config, readStream);
                        if (info != null)
                            return true;
                    }
                }
            }
            info = default;
            return false;
        }

        public static ImageInfo Identify(
            ImagingConfig config,
            ImageReadStream stream)
        {
            var format = DetectFormat(config, stream);
            var infoDetector = GetInfoDetector(format);
            var info = infoDetector.Identify(config, stream);
            if (info != null)
                return info;
            throw new InvalidDataException();
        }

        public static ImageInfo Identify(
            ImagingConfig config,
            Stream stream, 
            CancellationToken? cancellationToken = null)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            using (var readStream = config.CreateReadStream(stream, cancellationToken))
                return Identify(config, readStream);
        }

        public static ImageInfo Identify(ImagingConfig config, Stream stream)
        {
            return Identify(config, stream, CancellationToken.None);
        }

        /*
        public static bool TryIdentify(
            IReadOnlyMemory<byte> data, ImagingConfig config,
            CancellationToken cancellation, out ImageInfo info)
        {
            if (TryDetectFormat(data, config, cancellation, out var format))
                if (TryGetDecoder(format, out var decoder))
                    return decoder.TryIdentify(data, config, cancellation, out info);

            info = default;
            return false;
        }
        */

        /*
        public static ImageInfo Identify(
            IReadOnlyMemory<byte> data, ImagingConfig config, CancellationToken cancellation)
        {
            var format = DetectFormat(data, config, cancellation);
            var decoder = GetDecoder(format);
            if (!decoder.TryIdentify(data, config, cancellation, out var info))
                throw new InvalidDataException();
            return info;
        }
        */

        #endregion
    }
}
