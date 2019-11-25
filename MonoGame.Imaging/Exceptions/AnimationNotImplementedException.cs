using System;

namespace MonoGame.Imaging.Coding
{
    /// <summary>
    /// Thrown when attempting to process multiple frames with an 
    /// <see cref="IImageCoder"/> that is missing <see cref="IAnimatedFormatAttribute"/>.
    /// </summary>
    public class AnimationNotImplementedException : ImagingException
    {
        /// <summary>
        /// Gets the image coder that has not implemented animation.
        /// </summary>
        public IImageCoder Coder { get; }
        
        public AnimationNotImplementedException(IImageCoder coder) : base(coder?.Format)
        {
            Coder = coder ?? throw new ArgumentNullException(nameof(coder));
        }

        public AnimationNotImplementedException(string message, IImageCoder coder) : base(message, coder?.Format)
        {
            Coder = coder ?? throw new ArgumentNullException(nameof(coder));
        }

        public AnimationNotImplementedException(
            string message, Exception inner, IImageCoder coder) : base(message, inner, coder?.Format)
        {
            Coder = coder ?? throw new ArgumentNullException(nameof(coder));
        }
    }
}
