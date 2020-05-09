using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework.Vector;
using MonoGame.Imaging.Codecs.Decoding;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region Load(Stream)

        public static async Task<Image?> LoadAsync(
            IImagingConfig config,
            Stream stream,
            VectorTypeInfo? preferredPixelType = null,
            DecoderOptions? decoderOptions = null,
            DecodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            var frames = await CreateDecoderEnumeratorAsync(
                config, stream, leaveOpen: true, cancellationToken);

            await using (frames)
            {
                frames.State.DecoderOptions = decoderOptions;
                frames.State.PreferredPixelType = preferredPixelType;
                frames.State.Progress += onProgress;

                if (await frames.MoveNextAsync())
                    return frames.Current;
            }
            return null;
        }

        public static Task<Image?> LoadAsync(
            Stream stream,
            VectorTypeInfo? preferredPixelType = null,
            DecoderOptions? decoderOptions = null,
            DecodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            return LoadAsync(
                ImagingConfig.Default,
                stream, preferredPixelType, decoderOptions, onProgress, cancellationToken);
        }

        #endregion

        #region Load<TPixel>(Stream)

        public static async Task<Image<TPixel>?> LoadAsync<TPixel>(
            IImagingConfig config,
            Stream stream,
            DecoderOptions? decoderOptions = null,
            DecodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
            where TPixel : unmanaged, IPixel
        {
            var preferredType = VectorTypeInfo.Get<TPixel>();

            var image = await LoadAsync(
                config, stream, preferredType, decoderOptions, onProgress, cancellationToken);

            return (Image<TPixel>?)image;
        }

        public static Task<Image<TPixel>?> LoadAsync<TPixel>(
            Stream stream,
            DecoderOptions? decoderOptions = null,
            DecodeProgressCallback? onProgress = null,
            CancellationToken cancellationToken = default)
            where TPixel : unmanaged, IPixel
        {
            return LoadAsync<TPixel>(
                ImagingConfig.Default, stream, decoderOptions, onProgress, cancellationToken);
        }

        #endregion

        public Stream OpenReadStream(string filePath)
        {
            // All Stb decoders read file sequentially, so there's nothing to lose.
            var options = FileOptions.Asynchronous | FileOptions.SequentialScan;

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