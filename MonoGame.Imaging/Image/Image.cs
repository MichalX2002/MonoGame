using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Imaging.Decoding;
using MonoGame.Imaging.Encoding;
using MonoGame.Imaging.Utilities;
using MonoGame.Utilities;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Helper for detecting image formats and identifying and creating 
    /// images by decoding a stream or memory, copying memory, or wrapping around memory.
    /// </summary>
    public static partial class Image
    {
        // TODO: fix up this mess of coder lists

        private static List<IImageDecoder> _decoders;
        private static Dictionary<ImageFormat, IImageDecoder> _decoderByFormat;

        private static List<IImageEncoder> _encoders;
        private static Dictionary<ImageFormat, IImageEncoder> _encoderByFormat;

        private static List<IImageDecoder> GetDecoders()
        {
            if (_decoders == null)
            {
                _decoders = new List<IImageDecoder>();
                _decoders.Add(new BmpDecoder());
                _decoders.Add(new PngDecoder());
            }
            return _decoders;
        }

        private static List<IImageEncoder> GetEncoders()
        {
            if (_encoders == null)
            {
                _encoders = new List<IImageEncoder>();
                _encoders.Add(new PngEncoder());
            }
            return _encoders;
        }

        public static EncoderConfig GetDefaultEncoderConfig(ImageFormat format)
        {
            return GetEncoder(format).DefaultConfig;
        }

        #region [Try]GetDecoder

        public static bool TryGetDecoder(ImageFormat format, out IImageDecoder decoder)
        {
            if (_decoderByFormat == null)
            {
                _decoderByFormat = new Dictionary<ImageFormat, IImageDecoder>
                {
                    { GetDecoders()[0].Format, GetDecoders()[0] },
                    { GetDecoders()[1].Format, GetDecoders()[1] }
                };
            }
            return _decoderByFormat.TryGetValue(format, out decoder);
        }

        public static IImageDecoder GetDecoder(ImageFormat format)
        {
            if (TryGetDecoder(format, out var decoder))
                return decoder;
            throw new MissingDecoderException(format);
        }

        #endregion

        #region [Try]GetEncoder

        public static bool TryGetEncoder(ImageFormat format, out IImageEncoder encoder)
        {
            if (_encoderByFormat == null)
            {
                _encoderByFormat = new Dictionary<ImageFormat, IImageEncoder>
                {
                    { GetEncoders()[0].Format, GetEncoders()[0] }
                };
            }
            return _encoderByFormat.TryGetValue(format, out encoder);
        }

        public static IImageEncoder GetEncoder(ImageFormat format)
        {
            if (TryGetEncoder(format, out var encoder))
                return encoder;
            throw new MissingEncoderException(format);
        }

        #endregion

        #region WrapMemory

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            Memory<TPixel> memory, int width, int height, int stride)
            where TPixel : unmanaged, IPixel
        {
            CommonArgumentGuard.AssertAboveZero(width, nameof(width));
            CommonArgumentGuard.AssertAboveZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Length, width * height, nameof(memory));

            var buffer = new Image<TPixel>.Buffer(memory, stride);
            return new Image<TPixel>(buffer, width, height);
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            Memory<TPixel> memory, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory(memory, width, height, width * sizeof(TPixel));
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            IMemory<TPixel> memory, int width, int height, int stride, bool leaveOpen)
            where TPixel : unmanaged, IPixel
        {
            CommonArgumentGuard.AssertAboveZero(width, nameof(width));
            CommonArgumentGuard.AssertAboveZero(height, nameof(height));
            ImagingArgumentGuard.AssertContigousLargeEnough(memory.Span.Length, width * height, nameof(memory));

            var buffer = new Image<TPixel>.Buffer(memory, stride, leaveOpen);
            return new Image<TPixel>(buffer, width, height);
        }

        public static unsafe Image<TPixel> WrapMemory<TPixel>(
            IMemory<TPixel> memory, int width, int height, bool leaveOpen)
            where TPixel : unmanaged, IPixel
        {
            return WrapMemory(memory, width, height, width * sizeof(TPixel), leaveOpen);
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates an empty image using the
        /// <see cref="Image{TPixel}.Image(int, int)"/> constructor.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static unsafe Image<TPixel> Create<TPixel>(int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return new Image<TPixel>(width, height);
        }

        #endregion

        #region [Try]DetectFormat

        private static bool TryDetectFormat(ImageReadStream stream, out ImageFormat format)
        {
            foreach (var decoder in GetDecoders())
            {
                if (decoder.TryDetectFormat(stream, out format))
                    return true;
            }
            format = default;
            return false;
        }

        public static bool TryDetectFormat(Stream stream, out ImageFormat format)
        {
            using (var imageStream = new ImageReadStream(stream, true))
                return TryDetectFormat(imageStream, out format);
        }

        public static ImageFormat DetectFormat(Stream stream)
        {
            using (var imageStream = new ImageReadStream(stream, true))
                if (TryDetectFormat(imageStream, out var format))
                    return format;
            throw new UnknownImageFormatException();
        }

        public static bool TryDetectFormat(ReadOnlySpan<byte> data, out ImageFormat format)
        {
            if (!data.IsEmpty)
            {
                foreach (var decoder in GetDecoders())
                    if (decoder.TryDetectFormat(data, out format))
                        return true;
            }
            format = default;
            return false;
        }

        public static ImageFormat DetectFormat(ReadOnlySpan<byte> data)
        {
            if (TryDetectFormat(data, out var format))
                return format;
            throw new UnknownImageFormatException();
        }

        #endregion

        #region [Try]Identify

        public static bool TryIdentify(Stream stream, out ImageInfo info)
        {
            using (var imageStream = new ImageReadStream(stream, true))
            {
                if (TryDetectFormat(imageStream, out var format))
                    if (TryGetDecoder(format, out var decoder))
                        return decoder.TryIdentify(imageStream, out info);
            }
            info = default;
            return false;
        }

        public static ImageInfo Identify(Stream stream)
        {
            using (var imageStream = new ImageReadStream(stream, true))
            {
                var format = DetectFormat(imageStream);
                var decoder = GetDecoder(format);
                if (!decoder.TryIdentify(imageStream, out var info))
                    throw new InvalidDataException();
                return info;
            }
        }

        public static bool TryIdentify(ReadOnlySpan<byte> data, out ImageInfo info)
        {
            if (TryDetectFormat(data, out var format))
                if (TryGetDecoder(format, out var decoder))
                    return decoder.TryIdentify(data, out info);

            info = default;
            return false;
        }

        public static ImageInfo Identify(ReadOnlySpan<byte> data)
        {
            var format = DetectFormat(data);
            var decoder = GetDecoder(format);
            if (!decoder.TryIdentify(data, out var info))
                throw new InvalidDataException();
            return info;
        }

        #endregion
    }
}