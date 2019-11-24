using System;

namespace MonoGame.Imaging.Coding
{
    /// <summary>
    /// Allows processing of images with multiple frames sharing the same dimensions.
    /// </summary>
    public interface IAnimatedFormatAttribute : IImageFormatAttribute
    {
        /// <summary>
        /// Gets the minimum animation delay between frames.
        /// </summary>
        TimeSpan MinimumAnimationDelay { get; }
    }
}
