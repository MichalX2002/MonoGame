using System;

namespace MonoGame.Imaging
{
    public class AnimationNotSupportedException : Exception
    {
        /// <summary>
        /// Gets the format that does not support animation.
        /// </summary>
        public ImageFormat Format { get; }

        public AnimationNotSupportedException(ImageFormat format)
        {
            Format = format;
        }

        public AnimationNotSupportedException(ImageFormat format, string message) : base(message)
        {
            Format = format;
        }

        public AnimationNotSupportedException(
            ImageFormat format, string message, Exception inner) : base(message, inner)
        {
            Format = format;
        }
    }
}
