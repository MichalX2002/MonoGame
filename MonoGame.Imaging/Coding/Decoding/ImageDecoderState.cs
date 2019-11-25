using System;
using System.Threading;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Coding.Decoding
{
    public class ImageDecoderState<TPixel> where TPixel : unmanaged, IPixel
    {
        public ImageReadStream Stream { get; }

        public CancellationToken CancellationToken => Stream.Context.CancellationToken;

        /// <summary>
        /// Gets the decoder that the state originates from.
        /// </summary>
        public IImageDecoder Decoder { get; }

        /// <summary>
        /// Gets the most recently decoded image. 
        /// </summary>
        public Image<TPixel> Current { get; }

        /// <summary>
        /// Gets whether the decoder expects another frame from the stream.
        /// </summary>
        public bool HasNext { get; protected set; }

        public ImageDecoderState(
            ImageReadStream stream, 
            IImageDecoder decoder,
            Image<TPixel> current,
            bool hasNext)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            Current = current;
            HasNext = hasNext;
        }
    }
}
