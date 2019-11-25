using System;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;
using static StbSharp.StbImageWrite;

namespace MonoGame.Imaging.Encoding
{
    public abstract partial class StbEncoderBase : IImageEncoder
    {
        static StbEncoderBase()
        {
            ZlibCompress.CustomDeflateCompress = CustomDeflateCompress;
        }

        public abstract ImageFormat Format { get; }
        public abstract EncoderOptions DefaultOptions { get; }

        public virtual bool ImplementsAnimation => false;
        public virtual bool SupportsCancellation => true;

        // TODO: FIXME: properly handle ImageCollection type

        public void Encode<TPixel>(
            ImageCollection<TPixel, ReadOnlyImageFrame<TPixel>> images, Stream stream,
            EncoderOptions encoderOptions, ImagingConfig config,
            CancellationToken cancellation,
            EncodeProgressCallback<TPixel, ReadOnlyImageFrame<TPixel>> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            CommonArgumentGuard.AssertNonEmpty(images?.Count, nameof(images));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (encoderOptions == null) throw new ArgumentNullException(nameof(encoderOptions));
            if (config == null) throw new ArgumentNullException(nameof(config));
            EncoderOptions.AssertTypeEqual(DefaultOptions, encoderOptions, nameof(encoderOptions));

            cancellation.ThrowIfCancellationRequested();

            byte[] writeBuffer = RecyclableMemoryManager.Default.GetBlock();
            byte[] scratchBuffer = RecyclableMemoryManager.Default.GetBlock();
            try
            {
                for (int i = 0; i < images.Count; i++)
                {
                    cancellation.ThrowIfCancellationRequested();

                    var image = images[i];
                    int components = 4; // TODO: change this so it's dynamic
                    var provider = new BufferPixelProvider<TPixel>(frame.Pixels, components);
                    var progressCallback = onProgress == null ? (WriteProgressCallback)null : (p) =>
                        onProgress.Invoke(i, images, p);
                    
                    var context = new WriteContext(
                        provider.Fill, provider.Fill, progressCallback,
                        image.Width, image.Height, components, 
                        stream, cancellation, writeBuffer, scratchBuffer);

                    if (i == 0)
                    {
                        if (!WriteFirst(context, image, encoderOptions, config))
                            throw new ImageCoderException(Format);
                    }
                    else if (!WriteNext(context, image, encoderOptions, config))
                    {
                        break;
                    }
                }
            }
            finally
            {
                RecyclableMemoryManager.Default.ReturnBlock(scratchBuffer);
                RecyclableMemoryManager.Default.ReturnBlock(writeBuffer);
            }
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
