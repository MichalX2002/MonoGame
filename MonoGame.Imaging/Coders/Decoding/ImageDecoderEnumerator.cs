using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace MonoGame.Imaging.Coders.Decoding
{
    public class ImageDecoderEnumerator : IEnumerator<Image>, IImagingConfigurable
    {
        public bool IsDisposed { get; private set; }
        
        public IImageDecoder Decoder { get; }
        public IImagingConfig ImagingConfig => Decoder.ImagingConfig;

        public Image Current => Decoder.CurrentImage ?? throw new InvalidOperationException();
        object? IEnumerator.Current => Current;

        public CancellationToken CancellationToken { get; }

        #region Constructors

        public ImageDecoderEnumerator(IImageDecoder decoder, CancellationToken cancellationToken = default)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            CancellationToken = cancellationToken;
        }

        #endregion

        public bool MoveNext()
        {
            Decoder.Decode(CancellationToken);

            return Decoder.CurrentImage != null;
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
                    Decoder.Dispose();
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
