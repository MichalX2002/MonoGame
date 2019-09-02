using System;

namespace MonoGame.Imaging
{
    public class ImageCoderException : Exception
    {
        /// <summary>
        /// Gets the format of the coder.
        /// </summary>
        public ImageFormat Format { get; }

        public ImageCoderException(ImageFormat format)
        {
            Format = format;
        }

        public ImageCoderException(ImageFormat format, string message) : base(message)
        {
            Format = format;
        }

        public ImageCoderException(ImageFormat format, string message, Exception inner) : base(message, inner)
        {
            Format = format;
        }
    }
}
