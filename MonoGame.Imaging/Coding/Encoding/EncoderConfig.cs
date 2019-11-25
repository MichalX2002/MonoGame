using System;
using System.Diagnostics;

namespace MonoGame.Imaging.Coding
{
    /// <summary>
    /// Base class for encoder configurations.
    /// </summary>
    [Serializable]
    public abstract class EncoderConfig
    {
        public static bool TypeEquals(EncoderConfig a, Type b) => a.GetType() == b;

        public static bool TypeEquals(EncoderConfig a, EncoderConfig b) => TypeEquals(a, b.GetType());

        [DebuggerHidden]
        public static void AssertTypeEqual(EncoderConfig required, EncoderConfig other, string argName)
        {
            if(!TypeEquals(required, other))
                throw new ArgumentException(
                    $"The encoder configuration is not of the required type '{required.GetType()}'.", argName);
        }
    }
}
