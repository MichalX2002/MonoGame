using System;
using System.IO;
using System.Threading;
using MonoGame.Framework.Vectors;
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
            DecodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            var frames = CreateDecoderEnumerator(
                config, stream, leaveOpen: true, cancellationToken);

            using (frames)
            {
                frames.State.DecoderOptions = decoderOptions;
                frames.State.PreferredPixelType = preferredPixelType;
                frames.State.Progress += onProgress;

                if (frames.MoveNext())
                    return frames.Current;
            }
            return null;
        }

        public static Image? Load(
            Stream stream,
            VectorType? preferredPixelType = null,
            DecoderOptions? decoderOptions = null,
            DecodeProgressCallback? onProgress = null,
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
            DecodeProgressCallback? onProgress = null,
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
            DecodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return Load<TPixel>(
                ImagingConfig.Default, stream, decoderOptions, onProgress, cancellationToken);
        }

        #endregion

        private static Stream OpenReadStream(string filePath)
        {
            // All Stb decoders read file sequentially, so there's nothing to lose.
            var options = FileOptions.SequentialScan;

            throw new NotImplementedException();
        }

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