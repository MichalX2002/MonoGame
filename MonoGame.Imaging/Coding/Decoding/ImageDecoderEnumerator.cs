using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderEnumerator : IEnumerable<Image>, IEnumerator<Image>, IImagingConfigProvider
    {
        private ImageReadStream _readStream;
        private DecodeProgressCallback _progressCallback;

        public ImagingConfig ImagingConfig { get; }
        public IImageDecoder Decoder { get; }

        public ImageDecoderState State { get; private set; }
        public Image Current => State.CurrentImage;
        object IEnumerator.Current => Current;

        #region Constructors

        public ImageDecoderEnumerator(
            ImagingConfig config,
            IImageDecoder decoder,
            ImageReadStream readStream,
            DecodeProgressCallback progressCallback = null)
        {
            ImagingConfig = config ?? throw new ArgumentNullException(nameof(config));
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _readStream = readStream ?? throw new ArgumentNullException(nameof(readStream));
            _progressCallback = progressCallback;
        }

        #endregion

        public bool MoveNext()
        {
            if (State == null)
                State = Decoder.DecodeFirst(ImagingConfig, _readStream, _progressCallback);
            else
                Decoder.DecodeNext(State, _progressCallback);

            if (State.CurrentImage != null)
                return true;
            return false;
        }

        public void Reset()
        {
            // TODO: consider implementation based on seekable streams
            throw new NotSupportedException();
        }

        IEnumerator<Image> IEnumerable<Image>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        #region IDisposable

        protected void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            State?.Dispose();
            State = null;

            _readStream?.Dispose();
            _readStream = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageDecoderEnumerator()
        {
            Dispose(false);
        }

        #endregion
    }
}
