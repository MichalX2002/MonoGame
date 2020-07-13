using System;
using MonoGame.Imaging.Coders;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Base class for encoder options.
    /// </summary>
    [Serializable]
    public class EncoderOptions : CoderOptions
    {
        public static new EncoderOptions Default { get; } = new DefaultEncoderOptions();

        private class DefaultEncoderOptions : EncoderOptions
        {
        }
    }
}
