using System;

namespace MonoGame.Imaging
{
    public class MissingDecoderException : ImageCoderException
    {
        public MissingDecoderException(ImageFormat format) : base(format)
        {
        }

        public MissingDecoderException(ImageFormat format, string message) : base(format, message)
        {
        }

        public MissingDecoderException(ImageFormat format, string message, Exception inner) : base(format, message, inner)
        {
        }
    }
}
