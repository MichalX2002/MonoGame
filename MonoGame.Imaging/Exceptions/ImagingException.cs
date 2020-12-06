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

        public ImagingException(string? message) : this(message, null, null)
        {
        }

        public ImagingException(string? message, Exception? inner) : this(message, inner, null)
        {
        }
        
        public ImagingException(ImageFormat? format) : this(null, null, format)
        {
        }

        public ImagingException(string? message, ImageFormat? format) : this(message, null, format)
        {
        }

        public ImagingException(string? message, Exception? inner, ImageFormat? format) :
            base(message ?? format?.ToString(), inner)
        {
            Format = format;
        }
    }
}
