using System;

namespace MonoGame.Imaging
{
    public class MissingDecoderException : ImagingException
    {
        public MissingDecoderException(ImageFormat format) : base(format)
        {
        }

        public MissingDecoderException(string message, ImageFormat format) : base(message, format)
        {
        }

        public MissingDecoderException(
            string message, Exception inner, ImageFormat format) : base(message, inner, format)
        {
        }
    }
}
