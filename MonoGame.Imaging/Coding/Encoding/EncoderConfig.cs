using System;
using System.Diagnostics;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Base class for different encoder configurations.
    /// </summary>
    [Serializable]
    public abstract class EncoderConfig
    {
        public static bool TypeEquals(EncoderConfig a, Type b)
        {
            return a.GetType() == b;
        }

        public static bool TypeEquals(EncoderConfig a, EncoderConfig b)
        {
            return TypeEquals(a, b.GetType());
        }

        [DebuggerHidden]
        public static void AssertTypeEqual(EncoderConfig required, EncoderConfig other, string argName)
        {
            if(!TypeEquals(required, other))
                throw new ArgumentException(
                    $"The encoder configuration is not of the required type '{required.GetType()}'.", argName);
        }
    }
}
