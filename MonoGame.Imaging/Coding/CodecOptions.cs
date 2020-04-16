using System;

namespace MonoGame.Imaging.Coding
{
    /// <summary>
    /// Base class for codec options.
    /// </summary>
    [Serializable]
    public abstract class CodecOptions
    {
        public static CodecOptions Default { get; } = new DefaultCodecOptions();

        public static bool TypeEquals(CodecOptions a, Type b)
        {
            return a.GetType() == b;
        }

        public static bool TypeEquals(CodecOptions a, CodecOptions b)
        {
            return TypeEquals(a, b.GetType());
        }

        public static void AssertTypeEqual(
            CodecOptions required, CodecOptions other, string argName)
        {
            if (!TypeEquals(required, other))
                throw new ArgumentException(
                    $"The codec options are not of the required type '{required.GetType()}'.", argName);
        }

        private class DefaultCodecOptions : CodecOptions
        {
        }
    }
}
