using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MonoGame.Imaging.Coders.Decoding
{
    public class ImageDecoderEnumerator : IEnumerator<Image>, IImagingConfigurable
    {
        public ImageDecoderState State { get; }
        public bool IsDisposed { get; private set; }
        
        public IImagingConfig Config => State.Config;

        public Image Current => State.CurrentImage!;
        object? IEnumerator.Current => Current;

        #region Constructors

        public ImageDecoderEnumerator(
            IImagingConfig config,
            IImageDecoder decoder,
            Stream stream,
            bool leaveOpen,
            CancellationToken cancellationToken = default)
        {
            if (decoder == null)
                throw new ArgumentNullException(nameof(decoder));

            State = decoder.CreateState(config, stream, leaveOpen, cancellationToken);
        }

        #endregion

        public bool MoveNext()
        {
            State.Decoder.Decode(State);

            return State.CurrentImage != null;
        }

        public void Reset()
        {
            // consider implementation for seekable streams
            throw new NotSupportedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    State.Dispose();
                }
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
