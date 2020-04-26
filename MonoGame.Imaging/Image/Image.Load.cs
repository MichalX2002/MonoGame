using System.IO;
using System.Linq;
using System.Threading;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Coding.Decoding;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region Load(Stream)

        public static Image Load(
            ImagingConfig config,
            Stream stream,
            out ImageFormat format,
            VectorTypeInfo preferredPixelType = null,
            DecoderOptions decoderOptions = null,
            DecodeProgressCallback onProgress = null,
            CancellationToken cancellationToken = default)
        {
            using (var frames = EnumerateFrames(
                config, stream, out format, cancellationToken))
            {
                frames.State.DecoderOptions = decoderOptions;
                frames.State.PreferredPixelType = preferredPixelType;
                frames.State.Progress += onProgress;

                return frames.First();
            }
        }

        public static Image Load(
            ImagingConfig config,
            Stream stream,
            VectorTypeInfo preferredPixelType = null,
            DecoderOptions decoderOptions = null,
            DecodeProgressCallback onProgress = null,
            CancellationToken cancellationToken = default)
        {
            return Load(
                config, stream, out _,
                preferredPixelType, decoderOptions, onProgress, cancellationToken);
        }

        public static Image Load(
            Stream stream,
            out ImageFormat format,
            VectorTypeInfo preferredPixelType = null,
            DecoderOptions decoderOptions = null,
            DecodeProgressCallback onProgress = null,
            CancellationToken cancellationToken = default)
        {
            return Load(
                ImagingConfig.Default, stream, out format,
                preferredPixelType, decoderOptions, onProgress, cancellationToken);
        }

        public static Image Load(
            Stream stream,
            VectorTypeInfo preferredPixelType = null,
            DecoderOptions decoderOptions = null,
            DecodeProgressCallback onProgress = null,
            CancellationToken cancellationToken = default)
        {
            return Load(
                stream, out _,
                preferredPixelType, decoderOptions, onProgress, cancellationToken);
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