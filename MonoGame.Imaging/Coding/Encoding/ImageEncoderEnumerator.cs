using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Coding.Encoding
{
    public class ImageEncoderEnumerator<TPixel> : 
        IEnumerable<IReadOnlyPixelBuffer<TPixel>>, IEnumerator<IReadOnlyPixelBuffer<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        private Stream _stream;
        private bool _leaveOpen;

        public IImageEncoder Encoder { get; }
        public EncoderConfig EncoderConfig { get; }

        public IReadOnlyPixelBuffer<TPixel> Current { get; private set; }
        object IEnumerator.Current => Current;

        #region Constructors

        public ImageEncoderEnumerator(
            IImageEncoder encoder, ImagingConfig config, Stream stream, bool leaveOpen)
        {
            Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveOpen = leaveOpen;
        }

        #endregion

        public bool MoveNext()
        {

        }

        // TODO: consider implementation based on seekable streams
        public void Reset() => throw new NotSupportedException();

        public void Dispose()
        {
            if (!_leaveOpen)
            {
                _stream?.Dispose();
                _stream = null;
            }
        }

        IEnumerator<IReadOnlyPixelBuffer<TPixel>> IEnumerable<IReadOnlyPixelBuffer<TPixel>>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
