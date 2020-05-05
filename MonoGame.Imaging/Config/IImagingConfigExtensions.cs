using System.Collections.Generic;
using System.IO;
using MonoGame.Framework.IO;
using MonoGame.Imaging.Codecs.Decoding;
using MonoGame.Imaging.Codecs.Encoding;
using MonoGame.Imaging.Config;

namespace MonoGame.Imaging
{
    public static class IImagingConfigExtensions
    {
        #region [Try]GetEncoder

        public static bool TryGetEncoder(
            this IImagingConfig config, ImageFormat format, out IImageEncoder? encoder)
        {
            var provider = config.GetModule<ImageCodecProvider<IImageEncoder>>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageCodecProvider<IImageEncoder>)}.");

            return provider.TryGetCodec(format, out encoder);
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
            var provider = config.GetModule<ImageCodecProvider<IImageDecoder>>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageCodecProvider<IImageDecoder>)}.");

            return provider.TryGetCodec(format, out decoder);
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
            var provider = config.GetModule<ImageCodecProvider<IImageInfoDetector>>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageCodecProvider<IImageInfoDetector>)}.");

            return provider.TryGetCodec(format, out infoDetector);
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
            var provider = config.GetModule<ImageFormatDetectorProvider>();
            if (provider == null)
                throw new ImagingException($"Missing {nameof(ImageFormatDetectorProvider)}.");

            return provider.GetMaxHeaderSize();
        }

        public static IEnumerable<IImageFormatDetector> GetFormatDetectors(this IImagingConfig config)
        {
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
