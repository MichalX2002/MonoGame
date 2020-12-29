using System;

namespace MonoGame.Imaging
{
    public class UnknownImageFormatException : ImagingException
    {
        public UnknownImageFormatException()
        {
        }

        public UnknownImageFormatException(string? message) : base(message)
        {
        }

        public UnknownImageFormatException(string? message, Exception? inner) : base(message, inner)
        {
        }
    }
}
