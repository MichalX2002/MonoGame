using System;
using System.Collections;
using System.Collections.Generic;
using MonoGame.Imaging.Decoding;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public class ImageDecoderEnumerable<TPixel> : IEnumerable<Image<TPixel>>, IDisposable
        where TPixel : unmanaged, IPixel
    {
        private Enumerator _enumerator;

        #region Constructors

        public ImageDecoderEnumerable(
            IImageDecoder decoder, ImagingConfig config, ImageReadStream readStream, bool leaveOpen)
        {
            _enumerator = new Enumerator(decoder, readStream, leaveOpen);
        }

        #endregion

        IEnumerator<Image<TPixel>> IEnumerable<Image<TPixel>>.GetEnumerator() => _enumerator;
        IEnumerator IEnumerable.GetEnumerator() => _enumerator;

        public void Dispose()
        {
            _enumerator?.Dispose();
            _enumerator = null;
        }

        #region Enumerator

        public class Enumerator : IEnumerator<Image<TPixel>>
        {
            private ImageReadStream _readStream;
            private bool _leaveOpen;

            public IImageDecoder Decoder { get; }
            
            public Image<TPixel> Current { get; private set; }
            object IEnumerator.Current => Current;

            public Enumerator(
                IImageDecoder decoder, ImageReadStream readStream, bool leaveOpen)
            {
                Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
                _readStream = readStream ?? throw new ArgumentNullException(nameof(readStream));
                _leaveOpen = leaveOpen;
            }

            public bool MoveNext()
            {

            }

            public void Reset() => throw new NotSupportedException();

            public void Dispose()
            {
                if (!_leaveOpen)
                {
                    _readStream?.Dispose();
                    _readStream = null;
                }
            }
        }

        #endregion
    }
}
