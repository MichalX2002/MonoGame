using System;
using System.IO;
using MonoGame.Framework;

namespace MonoGame.Imaging.Coding.Decoding
{
    /// <summary>
    /// Represents a progress update for image decoding.
    /// </summary>
    public delegate void DecodeProgressCallback(
        ImageDecoderState decoderState,
        double percentage,
        Rectangle? rectangle);

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
