using System;

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

        public new ImageReadStream Stream => (ImageReadStream)base.Stream;
        
        #endregion

        public ImageDecoderState(
            ImagingConfig imagingConfig,
            IImageDecoder decoder,
            ImageReadStream stream) : 
            base(imagingConfig, stream, leaveOpen: false)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
        }
    }
}
