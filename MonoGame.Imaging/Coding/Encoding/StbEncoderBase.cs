using System;
using System.IO;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Utilities;
using MonoGame.Utilities;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;
using static StbSharp.StbImageWrite;

namespace MonoGame.Imaging.Encoding
{
    // TODO: add cancellation token to Stb's WriteContext

    public abstract partial class StbEncoderBase : IImageEncoder
    {
        static StbEncoderBase()
        {
            CustomZlibDeflateCompress = CustomDeflateCompress;
        }

        public abstract ImageFormat Format { get; }
        public abstract EncoderConfig DefaultConfig { get; }

        public virtual bool ImplementsAnimation => false;

        public void Encode<TPixel>(
            ReadOnlyFrameCollection<TPixel> frames, Stream stream,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            CommonArgumentGuard.AssertNonEmpty(frames?.Count, nameof(frames));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (encoderConfig == null) throw new ArgumentNullException(nameof(encoderConfig));
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            EncoderConfig.AssertTypeEqual(DefaultConfig, encoderConfig, nameof(encoderConfig));

            byte[] writeBuffer = RecyclableMemoryManager.Default.GetBlock();
            byte[] scratchBuffer = RecyclableMemoryManager.Default.GetBlock();
            try
            {
                for (int i = 0; i < frames.Count; i++)
                {
                    var frame = frames[i];
                    int components = 4; // TODO: change this so it's dynamic
                    var provider = new ImagePixelProvider<TPixel>(frame.Pixels, components);
                    var progressCallback = onProgress == null ? (WriteProgressCallback)null : (p) =>
                    {
                        if (!onProgress.Invoke(i, frames, p))
                            throw new CoderInterruptedException(Format);
                    };

                    var context = new WriteContext(
                        provider.Fill, provider.Fill, progressCallback,
                        frame.Width, frame.Height, components, stream, writeBuffer, scratchBuffer);

                    if (i == 0)
                    {
                        if (!WriteFirst(context, frame, encoderConfig, imagingConfig))
                            throw new ImageCoderException(Format);
                    }
                    else if (!WriteNext(context, frame, encoderConfig, imagingConfig))
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
            in WriteContext context, ReadOnlyImageFrame<TPixel> frame,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig)
            where TPixel : unmanaged, IPixel;

        protected virtual bool WriteNext<TPixel>(
            in WriteContext context, ReadOnlyImageFrame<TPixel> frame,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, imagingConfig);
            return false;
        }
    }
}
