using System;
using MonoGame.Imaging.Coders;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Thrown when attempting to process multiple frames with an 
    /// <see cref="ImageFormat"/> that is missing <see cref="IAnimatedFormatAttribute"/>.
    /// </summary>
    public class AnimationNotSupportedException : ImagingException
    {
        public AnimationNotSupportedException(ImageFormat format) : base(format)
        {
        }

        public AnimationNotSupportedException(string message, ImageFormat format) : base(message, format)
        {
        }

        public AnimationNotSupportedException(
            string message, Exception inner, ImageFormat format) : base(message, inner, format)
        {
        }
    }
}
