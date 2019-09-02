using System;
using System.IO;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Utilities;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;
using StbSharp;

namespace MonoGame.Imaging.Encoding
{
    public abstract partial class StbEncoderBase : IImageEncoder
    {
        public abstract ImageFormat Format { get; }
        public abstract EncoderConfig DefaultConfig { get; }
        public virtual bool SupportsAnimation => false;

        static StbEncoderBase()
        {
            StbImageWrite.CustomZlibDeflateCompress = CustomDeflateCompress;
        }

        public void Encode<TPixel>(
            ReadOnlyFrameCollection<TPixel> frames, Stream stream,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig,
            EncodeProgressDelegate<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            if (frames == null) throw new ArgumentNullException(nameof(frames));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (encoderConfig == null) throw new ArgumentNullException(nameof(encoderConfig));
            if (imagingConfig == null) throw new ArgumentNullException(nameof(imagingConfig));
            if (frames.Count <= 0) throw new ArgumentEmptyException(nameof(frames));
            EncoderConfig.AssertTypeEqual(DefaultConfig, encoderConfig, nameof(encoderConfig));

            byte[] writeBuffer = RecyclableMemoryManager.Default.GetBlock();
            byte[] scratchBuffer = RecyclableMemoryManager.Default.GetBlock();
            try
            {
                for (int i = 0; i < frames.Count; i++)
                {
                    var frame = frames[i];
                    int components = 4;
                    var provider = new ImagePixelProvider<TPixel>(frame.Pixels, components);

                    var context = new StbImageWrite.WriteContext(provider.Fill, provider.Fill,
                        frame.Width, frame.Height, components, stream, writeBuffer, scratchBuffer);

                    bool successfulWrite = i == 0
                        ? WriteFirst(context, frame, encoderConfig, imagingConfig, onProgress)
                        : WriteNext(context, frame, encoderConfig, imagingConfig, onProgress);

                    if (!successfulWrite)
                        throw new ImageCoderException(Format);
                }
            }
            finally
            {
                RecyclableMemoryManager.Default.ReturnBlock(scratchBuffer);
                RecyclableMemoryManager.Default.ReturnBlock(writeBuffer);
            }
        }

        protected abstract bool WriteFirst<TPixel>(
            in StbImageWrite.WriteContext context, ReadOnlyImageFrame<TPixel> frame,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig,
            EncodeProgressDelegate<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel;

        protected virtual bool WriteNext<TPixel>(
            in StbImageWrite.WriteContext context, ReadOnlyImageFrame<TPixel> frame,
            EncoderConfig encoderConfig, ImagingConfig imagingConfig,
            EncodeProgressDelegate<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertAnimationSupported(this);
            throw new NotImplementedException();
        }
    }
}
