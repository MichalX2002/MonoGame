using System;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using static StbSharp.StbImageWrite;

namespace MonoGame.Imaging.Coding.Encoding
{
    public abstract partial class StbEncoderBase : IImageEncoder
    {
        static StbEncoderBase()
        {
            ZlibCompress.CustomDeflateCompress = CustomDeflateCompress;
        }

        public abstract ImageFormat Format { get; }
        public abstract EncoderOptions DefaultOptions { get; }

        // TODO: FIXME: properly handle ImageCollection type

        public void Encode<TPixel>(
            ImageEncoderEnumerator<TPixel> images, Stream stream,
            EncoderOptions encoderOptions, ImagingConfig config,
            CancellationToken cancellation,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (encoderOptions == null) throw new ArgumentNullException(nameof(encoderOptions));
            if (config == null) throw new ArgumentNullException(nameof(config));
            EncoderOptions.AssertTypeEqual(DefaultOptions, encoderOptions, nameof(encoderOptions));

            cancellation.ThrowIfCancellationRequested();

            IReadOnlyPixelBuffer<TPixel> image = null;
            if (images.MoveNext())
                image = images.Current;
            else
                return;

            int index = 0;
            while (image != null)
            {
                byte[] writeBuffer = RecyclableMemoryManager.Default.GetBlock();
                byte[] scratchBuffer = RecyclableMemoryManager.Default.GetBlock();
                try
                {
                    int components = 4; // TODO: change this so it's dynamic
                    var provider = new BufferPixelProvider<TPixel>(image, components);
                    var progressCallback = onProgress == null ? (WriteProgressCallback)null : (p) =>
                        onProgress.Invoke(index, image, p);

                    var context = new WriteContext(
                        provider.Fill, provider.Fill, progressCallback,
                        image.Width, image.Height, components,
                        stream, cancellation, writeBuffer, scratchBuffer);

                    cancellation.ThrowIfCancellationRequested();

                    if (i == 0)
                    {
                        if (!WriteFirst(context, image, encoderOptions, config))
                            throw new ImageCoderException(Format);
                    }
                    else if (!WriteNext(context, image, encoderOptions, config))
                    {
                        break;
                    }
                    index++;
                }
                finally
                {
                    RecyclableMemoryManager.Default.ReturnBlock(scratchBuffer);
                    RecyclableMemoryManager.Default.ReturnBlock(writeBuffer);
                }
            }
        }

        public void EncodeFirst<TPixel>(
            IReadOnlyPixelBuffer<TPixel> image, Stream stream,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig,
            CancellationToken cancellationToken,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
        }

        public void EncodeNext<TPixel>(
            IReadOnlyPixelBuffer<TPixel> image, Stream stream,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig,
            CancellationToken cancellationToken, 
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
        }

        protected abstract bool WriteFirst<TPixel>(
            in WriteContext context, ReadOnlyImageFrame<TPixel> image,
            EncoderOptions encoderOptions, ImagingConfig config)
            where TPixel : unmanaged, IPixel;

        protected virtual bool WriteNext<TPixel>(
            in WriteContext context, ReadOnlyImageFrame<TPixel> image,
            EncoderOptions encoderOptions, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, config);
            return false;
        }
    }
}
