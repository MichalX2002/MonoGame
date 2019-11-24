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
        public abstract EncoderConfig DefaultConfig { get; }

        public virtual bool ImplementsAnimation => false;
        public virtual bool SupportsCancellation => true;

        // TODO: FIXME: properly handle ImageCollection type

        public void Encode<TPixel>(
            ImageCollection<TPixel, ReadOnlyImageFrame<TPixel>> images, Stream stream,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig,
            CancellationToken cancellation,
            EncodeProgressCallback<TPixel, ReadOnlyImageFrame<TPixel>> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            CommonArgumentGuard.AssertNonEmpty(images?.Count, nameof(images));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (encoderConfig == null) throw new ArgumentNullException(nameof(encoderConfig));
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            EncoderConfig.AssertTypeEqual(DefaultConfig, encoderConfig, nameof(encoderConfig));

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
                        if (!WriteFirst(context, image, encoderConfig, imagingConfig))
                            throw new ImageCoderException(Format);
                    }
                    else if (!WriteNext(context, image, encoderConfig, imagingConfig))
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
            EncoderConfig encoderConfig, ImagingConfig imagingConfig)
            where TPixel : unmanaged, IPixel;

        protected virtual bool WriteNext<TPixel>(
            in WriteContext context, ReadOnlyImageFrame<TPixel> image,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, imagingConfig);
            return false;
        }
    }
}
