using System;
using System.IO;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Coding.Encoding
{
    public class ImageEncoderState<TPixel> : ImageCoderState<TPixel>
        where TPixel : unmanaged, IPixel
    {
        #region Auto Properties

        /// <summary>
        /// Gets the encoder that the state originates from.
        /// </summary>
        public IImageEncoder Encoder { get; }

        /// <summary>
        /// Gets or sets the most recently encoded pixel buffer. 
        /// </summary>
        public IReadOnlyPixelBuffer<TPixel> Current { get; set; }

        #endregion

        public ImageEncoderState(
            IImageEncoder encoder, Stream stream, bool leaveOpen) : base(stream, leaveOpen)
        {
            Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }
    }
}
