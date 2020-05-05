using System;

namespace MonoGame.Imaging.Codecs
{
    /// <summary>
    /// Thrown when attempting to process multiple frames with an 
    /// <see cref="IImageCodec"/> that is missing <see cref="IAnimatedFormatAttribute"/>.
    /// </summary>
    public class AnimationNotImplementedException : ImagingException
    {
        /// <summary>
        /// Gets the image codec that has not implemented animation.
        /// </summary>
        public IImageCodec Codec { get; }
        
        public AnimationNotImplementedException(IImageCodec codec) : base(codec.Format)
        {
            Codec = codec ?? throw new ArgumentNullException(nameof(codec));
        }

        public AnimationNotImplementedException(string message, IImageCodec codec) : base(message, codec.Format)
        {
            Codec = codec ?? throw new ArgumentNullException(nameof(codec));
        }

        public AnimationNotImplementedException(
            string message, Exception inner, IImageCodec codec) : base(message, inner, codec.Format)
        {
            Codec = codec ?? throw new ArgumentNullException(nameof(codec));
        }
    }
}
