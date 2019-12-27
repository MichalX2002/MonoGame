using System;
using System.IO;
using System.Threading;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;
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

        // TODO: FIXME: make this into an extension method for IImageEncoder

        public void Encode<TPixel>(
            ImageEncoderEnumerator<TPixel> images,
            Stream stream,
            EncoderOptions encoderOptions, 
            ImagingConfig config,
            CancellationToken cancellation,
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {

            IReadOnlyPixelBuffer<TPixel> image = null;
            if (images.MoveNext())
                image = images.Current;
            else
                return;

            cancellation.ThrowIfCancellationRequested();

            int index = 0;
            while (image != null)
            {
                byte[] writeBuffer = RecyclableMemoryManager.Default.GetBlock();
                try
                {
                    int components = 4; // TODO: change this so it's dynamic/controlled
                    var provider = new BufferPixelProvider<TPixel>(image, components);
                    var state = new ImageEncoderState<TPixel>(this, stream, encoderOptions.LeaveStreamOpen);
                    var progressCallback = onProgress == null ? (WriteProgressCallback)null : (p) =>
                        onProgress.Invoke(state, p, null);

                    int bufferOffset = writeBuffer.Length / 2;
                    int scratchBufferLength = writeBuffer.Length - bufferOffset;

                    var context = new WriteContext(
                        provider.Fill, provider.Fill, progressCallback,
                        image.Width, image.Height, components,
                        stream, cancellation, 
                        new ArraySegment<byte>(writeBuffer, 0, bufferOffset), 
                        new ArraySegment<byte>(writeBuffer, bufferOffset, scratchBufferLength));

                    cancellation.ThrowIfCancellationRequested();

                    if (index == 0)
                    {
                        if (!WriteFirst(context, image, encoderOptions, config))
                            throw new ImagingException(Format);
                    }
                    else if (!WriteNext(context, image, encoderOptions, config))
                    {
                        break;
                    }
                    index++;
                }
                finally
                {
                    RecyclableMemoryManager.Default.ReturnBlock(writeBuffer);
                }
            }
        }

        #region IImageEncoder

        public ImageEncoderState<TPixel> EncodeFirst<TPixel>(
            IReadOnlyPixelBuffer<TPixel> image,
            Stream stream, 
            EncoderOptions encoderOptions, 
            ImagingConfig config, 
            CancellationToken cancellationToken, 
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (image == null) throw new ArgumentNullException(nameof(image));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ValidateParams(encoderOptions, config);

            GetBuffers(out byte[] writeBuffer, out byte[] scratchBuffer);
            try
            {

            }
            finally
            {
                FreeBuffers(writeBuffer, scratchBuffer);
            }
        }

        public bool EncodeNext<TPixel>(
            IReadOnlyPixelBuffer<TPixel> image, 
            ImageEncoderState<TPixel> encoderState,
            EncoderOptions encoderOptions,
            ImagingConfig config, 
            CancellationToken cancellationToken, 
            EncodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (image == null) throw new ArgumentNullException(nameof(image));
            if (encoderState == null) throw new ArgumentNullException(nameof(encoderState));
            ValidateParams(encoderOptions, config);

            GetBuffers(out byte[] writeBuffer, out byte[] scratchBuffer);
            try
            {

            }
            finally
            {
                FreeBuffers(writeBuffer, scratchBuffer);
            }
        }

        public virtual void FinishState<TPixel>(ImageEncoderState<TPixel> encoderState)
            where TPixel : unmanaged, IPixel
        {
        }

        #endregion

        private void ValidateParams(EncoderOptions encoderOptions, ImagingConfig config)
        {
            if (encoderOptions == null) throw new ArgumentNullException(nameof(encoderOptions));
            if (config == null) throw new ArgumentNullException(nameof(config));
            EncoderOptions.AssertTypeEqual(DefaultOptions, encoderOptions, nameof(encoderOptions));
        }

        private void GetBuffers(out byte[] writeBuffer, out byte[] scratchBuffer)
        {
            writeBuffer = RecyclableMemoryManager.Default.GetBlock();
            scratchBuffer = RecyclableMemoryManager.Default.GetBlock();
        }

        private void FreeBuffers(byte[] writeBuffer, byte[] scratchBuffer)
        {
            RecyclableMemoryManager.Default.ReturnBlock(scratchBuffer);
            RecyclableMemoryManager.Default.ReturnBlock(writeBuffer);
        }

        protected abstract bool WriteFirst<TPixel>(
            in WriteContext context,
            IReadOnlyPixelBuffer<TPixel> image,
            EncoderOptions encoderOptions, 
            ImagingConfig config)
            where TPixel : unmanaged, IPixel;

        protected virtual bool WriteNext<TPixel>(
            in WriteContext context, 
            IReadOnlyPixelBuffer<TPixel> image,
            EncoderOptions encoderOptions, 
            ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, config);
            return false;
        }
    }
}
