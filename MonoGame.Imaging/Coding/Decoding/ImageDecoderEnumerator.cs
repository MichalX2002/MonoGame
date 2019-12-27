using System;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderEnumerator<TPixel> : ImageCoderEnumerator<TPixel, Image<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        private ImageReadStream _readStream;
        private bool _leaveOpen;

        public IImageDecoder Decoder { get; }

        #region Constructors

        public ImageDecoderEnumerator(
            IImageDecoder decoder, ImagingConfig config, ImageReadStream readStream, bool leaveOpen)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _readStream = readStream ?? throw new ArgumentNullException(nameof(readStream));
            _leaveOpen = leaveOpen;
        }

        #endregion

        public override bool MoveNext()
        {
        }

        // TODO: consider implementation based on seekable streams
        public override void Reset() => throw new NotSupportedException();

        public override void Dispose()
        {
            if (!_leaveOpen)
            {
                _readStream?.Dispose();
                _readStream = null;
            }
        }
    }
}
