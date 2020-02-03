using System;
using System.IO;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Coding.Encoding
{
    public class ImageEncoderEnumerator : ImageCoderEnumerator<IReadOnlyPixelBuffer>
    {
        private Stream _stream;
        private bool _leaveOpen;

        public IImageEncoder Encoder { get; }
        public EncoderOptions EncoderOptions { get; }

        #region Constructor

        public ImageEncoderEnumerator(
            IImageEncoder encoder, Stream stream, bool leaveOpen)
        {
            Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveOpen = leaveOpen;
        }

        #endregion

        public override bool MoveNext()
        {

        }

        public override void Reset()
        {
            // TODO: consider implementation for seekable streams
            throw new NotSupportedException();
        }

        public override void Dispose()
        {
            if (!_leaveOpen)
            {
                _stream?.Dispose();
                _stream = null;
            }
        }
    }
}
