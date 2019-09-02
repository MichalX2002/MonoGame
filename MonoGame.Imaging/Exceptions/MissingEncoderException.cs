using System;

namespace MonoGame.Imaging
{
    public class MissingEncoderException : ImageCoderException
    {
        public MissingEncoderException(ImageFormat format) : base(format)
        {
        }

        public MissingEncoderException(ImageFormat format, string message) : base(format, message)
        {
        }

        public MissingEncoderException(ImageFormat format, string message, Exception inner) : base(format, message, inner)
        {
        }
    }
}