using System;
using System.Collections;
using System.Collections.Generic;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderEnumerator<TPixel> : IEnumerable<Image<TPixel>>, IEnumerator<Image<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        private ImageReadStream _readStream;
        private bool _leaveOpen;

        public IImageDecoder Decoder { get; }

        public Image<TPixel> Current { get; private set; }
        object IEnumerator.Current => Current;

        #region Constructors

        public ImageDecoderEnumerator(
            IImageDecoder decoder, ImagingConfig config, ImageReadStream readStream, bool leaveOpen)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _readStream = readStream ?? throw new ArgumentNullException(nameof(readStream));
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
                _readStream?.Dispose();
                _readStream = null;
            }
        }

        IEnumerator<Image<TPixel>> IEnumerable<Image<TPixel>>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
