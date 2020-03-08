using System;
using System.IO;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract class ImageDecoderState : ImageCodecState
    {
        #region Properties

        /// <summary>
        /// Gets the decoder that the state originates from.
        /// </summary>
        public IImageDecoder Decoder { get; }

        public Image CurrentImage { get; protected set; }

        #endregion

        public ImageDecoderState(
            ImagingConfig imagingConfig,
            IImageDecoder decoder,
            Stream stream) : 
            base(imagingConfig, stream, leaveOpen: false)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
        }
    }
}
