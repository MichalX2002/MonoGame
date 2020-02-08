using System;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderEnumerator : ImageCoderEnumerator<Image>
    {
        private ImageReadStream _readStream;
        private bool _leaveOpen;

        public IImageDecoder Decoder { get; }

        private DecodeProgressCallback ProgressCallback { get; }

        #region Constructors

        public ImageDecoderEnumerator(
            ImagingConfig imagingConfig,
            IImageDecoder decoder,
            DecodeProgressCallback progressCallback,
            ImageReadStream readStream,
            bool leaveOpen) : base(imagingConfig)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            ProgressCallback = progressCallback;
            _readStream = readStream ?? throw new ArgumentNullException(nameof(readStream));
            _leaveOpen = leaveOpen;
        }

        #endregion

        public override bool MoveNext()
        {
            if(ImageIndex == -1)
            {
                Decoder.DecodeFirst()
            }
            else
            {

            }
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
