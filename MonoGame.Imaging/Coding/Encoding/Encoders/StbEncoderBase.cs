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

        // TODO: FIXME: make this into an extension method for IImageEncoder

        public void Encode(
            ImagingConfig imagingConfig,
            ImageEncoderEnumerator images,
            Stream stream,
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null,
            EncodeProgressCallback onProgress = null)
        {

            IReadOnlyPixelBuffer image = null;
            if (images.MoveNext())
                image = images.Current;
            else
                return;

            cancellationToken?.ThrowIfCancellationRequested();

            int index = 0;
            while (image != null)
            {
                byte[] writeBuffer = RecyclableMemoryManager.Default.GetBlock();
                try
                {
                    int components = 4; // TODO: change this so it's dynamic/controlled
                    var provider = new BufferPixelProvider(image, components);
                    var state = new ImageEncoderState(this, imagingConfig, stream, encoderOptions.LeaveStreamOpen);
                    var progressCallback = onProgress == null ? (WriteProgressCallback)null : (p) =>
                        onProgress.Invoke(state, p, null);

                    int bufferOffset = writeBuffer.Length / 2;
                    int scratchBufferLength = writeBuffer.Length - bufferOffset;

                    var context = new WriteContext(
                        provider.Fill, provider.Fill, progressCallback,
                        image.Width, image.Height, components,
                        stream, cancellationToken, 
                        new ArraySegment<byte>(writeBuffer, 0, bufferOffset), 
                        new ArraySegment<byte>(writeBuffer, bufferOffset, scratchBufferLength));

                    cancellationToken?.ThrowIfCancellationRequested();

                    if (index == 0)
                    {
                        if (!WriteFirst(imagingConfig, context, image, encoderOptions))
                            throw new ImagingException(Format);
                    }
                    else if (!WriteNext(imagingConfig, context, image, encoderOptions))
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

        public ImageEncoderState EncodeFirst(
            ImagingConfig config,
            IReadOnlyPixelBuffer image,
            Stream stream, 
            EncoderOptions encoderOptions = null, 
            CancellationToken? cancellationToken = null, 
            EncodeProgressCallback onProgress = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();

            if (config == null) throw new ArgumentNullException(nameof(config));
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ValidateEncoderOptions(encoderOptions);

            GetBuffers(out byte[] writeBuffer, out byte[] scratchBuffer);
            try
            {

            }
            finally
            {
                FreeBuffers(writeBuffer, scratchBuffer);
            }
        }

        public bool EncodeNext(
            ImageEncoderState encoderState,
            IReadOnlyPixelBuffer image, 
            EncoderOptions encoderOptions = null,
            CancellationToken? cancellationToken = null, 
            EncodeProgressCallback onProgress = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();

            if (encoderState == null) throw new ArgumentNullException(nameof(encoderState));
            if (image == null) throw new ArgumentNullException(nameof(image));
            ValidateEncoderOptions(encoderOptions);

            GetBuffers(out byte[] writeBuffer, out byte[] scratchBuffer);
            try
            {

            }
            finally
            {
                FreeBuffers(writeBuffer, scratchBuffer);
            }
        }

        public virtual void FinishState(ImageEncoderState encoderState)
        {
        }

        #endregion

        private void ValidateEncoderOptions(EncoderOptions encoderOptions)
        {
            if (encoderOptions == null)
                throw new ArgumentNullException(nameof(encoderOptions));
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

        protected abstract bool WriteFirst(
            ImagingConfig imagingConfig,
            in WriteContext context,
            IReadOnlyPixelBuffer image,
            EncoderOptions encoderOptions);

        protected virtual bool WriteNext(
            ImagingConfig imagingConfig,
            in WriteContext context, 
            IReadOnlyPixelBuffer image,
            EncoderOptions encoderOptions)
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, imagingConfig);
            return false;
        }
    }
}
