using System;
using System.IO;
using System.Threading;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Coding;
using MonoGame.Imaging.Coding.Decoding;

namespace MonoGame.Imaging
{
    // TODO: FIXME: change return collection type on LoadFrames methods

    public static partial class Image
    {
        #region LoadFrames(Stream)

        public static ImageCollection<TPixel, ImageFrame<TPixel>> LoadFrames<TPixel>(
            Stream stream, ImagingConfig config, int? frameLimit, out ImageFormat format,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            using (var imageStream = config.CreateReadStream(stream, cancellation))
            {
                if (!TryDetectFormat(imageStream, config, out format))
                    throw new UnknownImageFormatException();

                if (!TryGetDecoder(format, out var decoder))
                    throw new MissingDecoderException(format);

                return decoder.Decode(imageStream, config, frameLimit, onProgress);
            }
        }

        public static ImageCollection<TPixel, ImageFrame<TPixel>> LoadFrames<TPixel>(
            Stream stream, int? frameLimit, out ImageFormat format,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(
                stream, ImagingConfig.Default, frameLimit, out format, cancellation, onProgress);
        }

        #endregion

        /* TODO: fix this :)

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


        #region Load(Stream)

        public static Image<TPixel> Load<TPixel>(
            Stream stream, ImagingConfig config, out ImageFormat format,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(stream, config, 1, out format, cancellation, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            Stream stream,
            ImagingConfig config,
            CancellationToken cancellation,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(stream, config, out _, cancellation, onProgress);
        }


        public static Image<TPixel> Load<TPixel>(
            Stream stream, 
            out ImageFormat format,
            CancellationToken cancellation, 
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(stream, ImagingConfig.Default, 1, out format, cancellation, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            Stream stream, 
            CancellationToken cancellation, 
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(stream, out _, cancellation, onProgress);
        }

        #endregion

        /* TODO: fix this :)

        #region Load(ReadOnlySpan)

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config, out ImageFormat format,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(data, config, 1, out format, cancellation, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(data, config, out _, cancellation, onProgress);
        }


        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, out ImageFormat format,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(
                data, ImagingConfig.Default, 1, out format, cancellation, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, CancellationToken cancellation, 
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(data, out _, cancellation, onProgress);
        }

        #endregion

        */
    }
}