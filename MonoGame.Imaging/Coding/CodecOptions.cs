using System;

namespace MonoGame.Imaging.Coding
{
    /// <summary>
    /// Base class for codec options.
    /// </summary>
    [Serializable]
    public class CodecOptions
    {
        public static CodecOptions Default { get; } = new DefaultCodecOptions();

        public bool IsAssignableFrom(CodecOptions other)
        {
            return GetType().IsAssignableFrom(other.GetType());
        }

        private class DefaultCodecOptions : CodecOptions
        {
        }
    }
}
