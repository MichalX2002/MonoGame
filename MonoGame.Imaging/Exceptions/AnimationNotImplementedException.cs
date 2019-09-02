using System;

namespace MonoGame.Imaging
{
    public class AnimationNotImplementedException : Exception
    {
        /// <summary>
        /// Gets the image coder that does not support animation.
        /// </summary>
        public IImageCoder Coder { get; }

        /// <summary>
        /// Gets the format of the <see cref="Coder"/>.
        /// </summary>
        public ImageFormat Format => Coder.Format;

        public AnimationNotImplementedException(IImageCoder coder)
        {
            Coder = coder;
        }

        public AnimationNotImplementedException(IImageCoder coder, string message) : base(message)
        {
            Coder = coder;
        }

        public AnimationNotImplementedException(
            IImageCoder coder, string message, Exception inner) : base(message, inner)
        {
            Coder = coder;
        }
    }
}
