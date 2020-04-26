using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderEnumerator : IEnumerable<Image>, IEnumerator<Image>, IImagingConfigProvider
    {
        public ImageDecoderState State { get; }
        public ImagingConfig Config => State.Config;

        public Image Current => State.CurrentImage;
        object IEnumerator.Current => Current;

        #region Constructors

        public ImageDecoderEnumerator(
            IImageDecoder decoder,
            ImagingConfig config,
            ImageReadStream stream)
        {
            if (decoder == null)
                throw new ArgumentNullException(nameof(decoder));
            State = decoder.CreateState(config, stream);
        }

        #endregion

        public bool MoveNext()
        {
            if (State.Decoder.Decode(State))
            {
                if (State.CurrentImage == null)
                    throw new Exception(
                        "The decoder reported a successful decode, but there is no resulting image.");
                return true;
            }

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
