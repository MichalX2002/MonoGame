using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Framework.IO;
using MonoGame.Imaging.Coders.Decoding;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Config;

namespace MonoGame.Imaging
{
    public static class IImagingConfigExtensions
    {
        #region [Try]GetEncoder

        public static bool TryGetEncoder(
            this IImagingConfig config, ImageFormat format, out IImageEncoder? encoder)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var provider = config.GetModule<ImageCoderProvider<IImageEncoder>>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageCoderProvider<IImageEncoder>)}.");

            return provider.TryGetCoder(format, out encoder);
        }

        public static IImageEncoder GetEncoder(this IImagingConfig config, ImageFormat format)
        {
            if (!TryGetEncoder(config, format, out var encoder))
                throw new MissingEncoderException(format);
            return encoder!;
        }

        #endregion

        #region [Try]GetDecoder

        public static bool TryGetDecoder(
            this IImagingConfig config, ImageFormat format, out IImageDecoder? decoder)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var provider = config.GetModule<ImageCoderProvider<IImageDecoder>>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageCoderProvider<IImageDecoder>)}.");

            return provider.TryGetCoder(format, out decoder);
        }

        public static IImageDecoder GetDecoder(this IImagingConfig config, ImageFormat format)
        {
            if (!TryGetDecoder(config, format, out var decoder))
                throw new MissingDecoderException(format);
            return decoder!;
        }

        #endregion

        #region [Try]GetInfoDetector

        public static bool TryGetInfoDetector(
            this IImagingConfig config, ImageFormat format, out IImageInfoDetector? infoDetector)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var provider = config.GetModule<ImageCoderProvider<IImageInfoDetector>>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageCoderProvider<IImageInfoDetector>)}.");

            return provider.TryGetCoder(format, out infoDetector);
        }

        public static IImageInfoDetector GetInfoDetector(this IImagingConfig config, ImageFormat format)
        {
            if (!TryGetInfoDetector(config, format, out var decoder))
                throw new MissingDecoderException(format);
            return decoder!;
        }

        #endregion

        public static int GetMaxHeaderSize(this IImagingConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var provider = config.GetModule<ImageFormatDetectorProvider>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageFormatDetectorProvider)}.");

            return provider.GetMaxHeaderSize();
        }

        public static IEnumerable<IImageFormatDetector> GetFormatDetectors(this IImagingConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var provider = config.GetModule<ImageFormatDetectorProvider>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageFormatDetectorProvider)}.");

            return provider.Values;
        }

        public static PrefixedStream CreateStreamWithHeaderPrefix(
            this IImagingConfig config, Stream stream, bool leaveOpen)
        {
            int readAhead = GetMaxHeaderSize(config);
            return new PrefixedStream(stream, readAhead, leaveOpen);
        }
    }
}
