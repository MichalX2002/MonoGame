using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MonoGame.Imaging.Encoding;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging
{
    public class ImageEncoderEnumerable<TPixel> : IEnumerable<IReadOnlyPixelBuffer<TPixel>>, IDisposable
        where TPixel : unmanaged, IPixel
    {
        private Enumerator _enumerator;

        #region Constructors

        public ImageEncoderEnumerable(
            IImageEncoder encoder, ImagingConfig config, Stream stream, bool leaveOpen)
        {
            _enumerator = new Enumerator(encoder, stream, leaveOpen);
        }

        #endregion

        IEnumerator<IReadOnlyPixelBuffer<TPixel>> IEnumerable<IReadOnlyPixelBuffer<TPixel>>.GetEnumerator() => _enumerator;
        IEnumerator IEnumerable.GetEnumerator() => _enumerator;

        public void Dispose()
        {
            _enumerator?.Dispose();
            _enumerator = null;
        }

        #region Enumerator

        public class Enumerator : IEnumerator<IReadOnlyPixelBuffer<TPixel>>
        {
            private Stream _stream;
            private bool _leaveOpen;

            public IImageEncoder Encoder { get; }
            public EncoderOptions EncoderOptions { get; }

            public IReadOnlyPixelBuffer<TPixel> Current { get; private set; }
            object IEnumerator.Current => Current;

            public Enumerator(
                IImageEncoder encoder, 
                Stream stream, bool leaveOpen)
            {
                Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
                _stream = stream ?? throw new ArgumentNullException(nameof(stream));
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
                    _stream?.Dispose();
                    _stream = null;
                }
            }
        }

        #endregion
    }
}
