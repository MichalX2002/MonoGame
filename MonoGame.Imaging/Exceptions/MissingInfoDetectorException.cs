using System;

namespace MonoGame.Imaging
{
    public class MissingInfoDetectorException : ImagingException
    {
        public MissingInfoDetectorException(ImageFormat format) : base(format)
        {
        }

        public MissingInfoDetectorException(string message, ImageFormat format) : base(message, format)
        {
        }

        public MissingInfoDetectorException(
            string message, Exception inner, ImageFormat format) : base(message, inner, format)
        {
        }
    }
}
