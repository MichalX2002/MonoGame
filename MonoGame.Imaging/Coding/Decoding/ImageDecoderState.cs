using System;
using System.Threading;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderState<TPixel> : ImageCoderState<TPixel>
        where TPixel : unmanaged, IPixel
    {
        #region Auto Properties

        /// <summary>
        /// Gets the decoder that the state originates from.
        /// </summary>
        public IImageDecoder Decoder { get; }

        /// <summary>
        /// Gets or sets the most recently decoded image. 
        /// </summary>
        public Image<TPixel> Current { get; set; }

        #endregion

        #region Properties

        public new ImageReadStream Stream => (ImageReadStream)base.Stream;

        public CancellationToken CancellationToken => Stream.Context.CancellationToken;

        #endregion

        public ImageDecoderState(
            IImageDecoder decoder, ImageReadStream stream, bool leaveOpen) : base(stream, leaveOpen)
        {
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
        }
    }
}
