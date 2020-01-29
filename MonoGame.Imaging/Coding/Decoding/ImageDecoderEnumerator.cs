using System;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderEnumerator<TPixel> : ImageCoderEnumerator<TPixel, Image<TPixel>>
        where TPixel : unmanaged, IPixel
    {
        private ImageReadStream _readStream;
        private bool _leaveOpen;

        private DecodeProgressCallback ProgressCallback { get; }

        public IImageDecoder Decoder { get; }

        #region Constructors

        public ImageDecoderEnumerator(
            IImageDecoder decoder,
            DecodeProgressCallback progressCallback,
            ImageReadStream readStream,
            bool leaveOpen)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            ProgressCallback = progressCallback;
            _readStream = readStream ?? throw new ArgumentNullException(nameof(readStream));
            _leaveOpen = leaveOpen;
        }

        #endregion

        public override bool MoveNext()
        {
        }

        public override void Reset()
        {
            // TODO: consider implementation based on seekable streams
            throw new NotSupportedException();
        }

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
