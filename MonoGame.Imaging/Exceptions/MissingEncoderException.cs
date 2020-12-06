using System;

namespace MonoGame.Imaging
{
    public class MissingEncoderException : ImagingException
    {
        public MissingEncoderException(ImageFormat format) : this(null, format)
        {
        }

        public MissingEncoderException(string? message, ImageFormat format) : this(message, null, format)
        {
        }

        public MissingEncoderException(
            string? message, Exception? inner, ImageFormat format) : base(message, inner, format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));
        }
    }
}