using System.IO;
using System.Threading;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Attributes.Coder;
using MonoGame.Imaging.Coders.Decoding;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region Load(Stream)

        public static Image? Load(
            IImagingConfig config,
            Stream stream,
            VectorType? preferredPixelType = null,
            DecoderOptions? decoderOptions = null,
            ImagingProgressCallback<IImageDecoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            using ImageDecoderEnumerator frames = CreateDecoderEnumerator(
                config, stream, decoderOptions, cancellationToken);

            frames.Decoder.TargetPixelType = preferredPixelType;

            if (onProgress != null && frames.Decoder is IProgressReportingCoder<IImageDecoder> progressReporter)
                progressReporter.Progress += onProgress;

            if (frames.MoveNext())
                return frames.Current;

            return null;
        }

        public static Image? Load(
            Stream stream,
            VectorType? preferredPixelType = null,
            DecoderOptions? decoderOptions = null,
            ImagingProgressCallback<IImageDecoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            return Load(
                ImagingConfig.Default,
                stream, preferredPixelType, decoderOptions, onProgress, cancellationToken);
        }

        #endregion

        #region Load<TPixel>(Stream)

        public static Image<TPixel>? Load<TPixel>(
            IImagingConfig config,
            Stream stream,
            DecoderOptions? decoderOptions = null,
            ImagingProgressCallback<IImageDecoder>? onProgress = null,
            CancellationToken cancellationToken = default)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var preferredType = VectorType.Get<TPixel>();

            var image = Load(
                config, stream, preferredType, decoderOptions, onProgress, cancellationToken);

            return (Image<TPixel>?)image;
        }

        public static Image<TPixel>? Load<TPixel>(
            Stream stream,
            DecoderOptions? decoderOptions = null,
            ImagingProgressCallback<IImageDecoder>? onProgress = null,
            CancellationToken cancellationToken = default)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return Load<TPixel>(
                ImagingConfig.Default, stream, decoderOptions, onProgress, cancellationToken);
        }

        #endregion

        #region Load(FilePath)

        public static Image? Load(
            IImagingConfig config,
            string filePath,
            VectorType? preferredPixelType = null,
            DecoderOptions? decoderOptions = null,
            ImagingProgressCallback<IImageDecoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            using var fs = File.OpenRead(filePath);
            return Load(config, fs, preferredPixelType, decoderOptions, onProgress, cancellationToken);
        }

        public static Image? Load(
            string filePath,
            VectorType? preferredPixelType = null,
            DecoderOptions? decoderOptions = null,
            ImagingProgressCallback<IImageDecoder>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            return Load(
                ImagingConfig.Default,
                filePath, preferredPixelType, decoderOptions, onProgress, cancellationToken);
        }

        #endregion

        #region Load<TPixel>(FilePath)

        public static Image<TPixel>? Load<TPixel>(
            IImagingConfig config,
            string filePath,
            DecoderOptions? decoderOptions = null,
            ImagingProgressCallback<IImageDecoder>? onProgress = null,
            CancellationToken cancellationToken = default)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using var fs = File.OpenRead(filePath);
            return Load<TPixel>(config, fs, decoderOptions, onProgress, cancellationToken);
        }

        public static Image<TPixel>? Load<TPixel>(
            string filePath,
            DecoderOptions? decoderOptions = null,
            ImagingProgressCallback<IImageDecoder>? onProgress = null,
            CancellationToken cancellationToken = default)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return Load<TPixel>(
                ImagingConfig.Default, filePath, decoderOptions, onProgress, cancellationToken);
        }

        #endregion

        /* TODO: fix this :)

        #region Load(ReadOnlySpan)

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config, out ImageFormat format,
            CancellationToken cancellation, CoderProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(data, config, 1, out format, cancellation, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config,
            CancellationToken cancellation, CoderProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(data, config, out _, cancellation, onProgress);
        }


        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, out ImageFormat format,
            CancellationToken cancellation, CoderProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return LoadFrames(
                data, ImagingConfig.Default, 1, out format, cancellation, onProgress)?.First.Pixels;
        }

        public static Image<TPixel> Load<TPixel>(
            ReadOnlySpan<byte> data, CancellationToken cancellation, 
            CoderProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            return Load(data, out _, cancellation, onProgress);
        }

        #endregion

        */
    }
}