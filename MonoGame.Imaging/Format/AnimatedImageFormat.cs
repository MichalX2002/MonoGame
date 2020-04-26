using System;
using MonoGame.Framework.Collections;
using MonoGame.Imaging.Attributes.Format;

namespace MonoGame.Imaging
{
    public class AnimatedImageFormat : ImageFormat, IAnimatedFormatAttribute
    {
        public TimeSpan MinimumAnimationDelay { get; }

        public AnimatedImageFormat(
            string fullName, string shortName,
            IReadOnlySet<string> mimeTypes,
            IReadOnlySet<string> extensions,
            TimeSpan minimumAnimationDelay) :
            base(fullName, shortName, mimeTypes, extensions)
        {
            MinimumAnimationDelay = minimumAnimationDelay.Duration();
        }
    }
}
