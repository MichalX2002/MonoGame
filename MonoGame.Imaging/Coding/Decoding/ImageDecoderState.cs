using System;
using System.Threading;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderState : ImageCoderState
    {
        #region Properties

        /// <summary>
        /// Gets the decoder that the state originates from.
        /// </summary>
        public IImageDecoder Decoder { get; }

        public Image CurrentImage { get; set; }

        public new ImageReadStream Stream => (ImageReadStream)base.Stream;

        public CancellationToken CancellationToken => Stream.Context.CancellationToken;

        #endregion

        public ImageDecoderState(
            IImageDecoder decoder,
            ImagingConfig imagingConfig, 
            ImageReadStream stream,
            bool leaveOpen) : base(imagingConfig, stream, leaveOpen)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
        }
    }
}
