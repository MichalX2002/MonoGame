using System;
using System.IO;
using MonoGame.Framework;
using MonoGame.Imaging.Decoding;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public static partial class Image
    {
        #region LoadFrames(Stream)

        public static FrameCollection<TPixel> LoadFrames<TPixel>(
            Stream stream, ImagingConfig config, int? frameLimit, out ImageFormat format,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            using (var imageStream = new ImageReadStream(stream, leaveOpen: true))
            {
                if (!TryDetectFormat(imageStream, out format))
                    throw new UnknownImageFormatException();

                if (!TryGetDecoder(format, out var decoder))
                    throw new ImageCoderException(format);

                return decoder.Decode(config, imageStream, frameLimit, onProgress);
            }
        }

        public static FrameCollection<TPixel> LoadFrames<TPixel>(
            Stream stream, int? frameLimit, out ImageFormat format,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(stream, ImagingConfig.Default, frameLimit, out format, onProgress);
        }

        #endregion

        #region LoadFrames(ReadOnlySpan)

        public static FrameCollection<TPixel> LoadFrames<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config, int? frameLimit, out ImageFormat format,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (data.IsEmpty) throw new ArgumentEmptyException(nameof(data));

            format = DetectFormat(data);
            var decoder = GetDecoder(format);
            return decoder.Decode(config, data, frameLimit, onProgress);
        }

        public static FrameCollection<TPixel> LoadFrames<TPixel>(
            ReadOnlySpan<byte> data, int? frameLimit, out ImageFormat format,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(data, ImagingConfig.Default, frameLimit, out format, onProgress);
        }

        #endregion


        #region Load(Stream)

        public static Image<TPixel> Load<TPixel>(
            Stream stream, ImagingConfig config, out ImageFormat format,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(stream, config, 1, out format, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            Stream stream, ImagingConfig config,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(stream, config, out _, onProgress);
        }


        public static Image<TPixel> Load<TPixel>(
            Stream stream, out ImageFormat format,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(stream, ImagingConfig.Default, 1, out format, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            Stream stream, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(stream, out _, onProgress);
        }

        #endregion

        #region Load(ReadOnlySpan)

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config, out ImageFormat format,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(data, config, 1, out format, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(data, config, out _, onProgress);
        }


        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, out ImageFormat format,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(data, ImagingConfig.Default, 1, out format, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(data, out _, onProgress);
        }

        #endregion
    }
}