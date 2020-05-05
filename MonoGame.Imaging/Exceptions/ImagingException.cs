using System;

namespace MonoGame.Imaging
{
    public class ImagingException : Exception
    {
        /// <summary>
        /// Gets the <see cref="ImageFormat"/> associated with the exception.
        /// </summary>
        public ImageFormat? Format { get; }

        public ImagingException()
        {
        }

        public ImagingException(string message) : base(message)
        {
        }

        public ImagingException(string message, Exception inner) : base(message, inner)
        {
        }
        
        public ImagingException(ImageFormat format)
        {
            Format = format;
        }

        public ImagingException(string message, ImageFormat format) : base(message)
        {
            Format = format;
        }

        public ImagingException(string message, Exception inner, ImageFormat format) : base(message, inner)
        {
            Format = format;
        }
    }
}
