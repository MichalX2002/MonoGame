using System;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Occurs when a <see cref="IImageCoder"/> is interrupted by a progress callback.
    /// </summary>
    public class CoderInterruptedException : ImageCoderException
    {
        public CoderInterruptedException(ImageFormat format) : base(format)
        {
        }

        public CoderInterruptedException(ImageFormat format, string message) : base(format, message)
        {
        }

        public CoderInterruptedException(ImageFormat format, string message, Exception inner) :
            base(format, message, inner)
        {
        }
    }
}
