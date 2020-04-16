using System;
using MonoGame.Imaging.Coding;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Base class for encoder options.
    /// </summary>
    [Serializable]
    public abstract class EncoderOptions : CodecOptions
    {
        public static new EncoderOptions Default { get; } = new DefaultEncoderOptions();

        private class DefaultEncoderOptions : EncoderOptions
        {
        }
    }
}
