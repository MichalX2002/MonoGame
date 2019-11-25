using System;

namespace MonoGame.Imaging
{
    public class MissingEncoderException : ImagingException
    {
        public MissingEncoderException(ImageFormat format) : base(format)
        {
        }

        public MissingEncoderException(string message, ImageFormat format) : base(message, format)
        {
        }

        public MissingEncoderException(
            string message, Exception inner, ImageFormat format) : base(message, inner, format)
        {
        }
    }
}