using System;
using System.Diagnostics;
using System.IO;

namespace MonoGame.Imaging
{
    /// <summary>
    /// Base class for encoder options.
    /// </summary>
    [Serializable]
    public abstract class EncoderOptions
    {
        public static bool TypeEquals(EncoderOptions a, Type b) => a.GetType() == b;

        public static bool TypeEquals(EncoderOptions a, EncoderOptions b) => TypeEquals(a, b.GetType());

        /// <summary>
        /// Gets whether coder operations should close the <see cref="Stream"/>.
        /// </summary>
        public bool LeaveStreamOpen { get; }

        [DebuggerHidden]
        public static void AssertTypeEqual(EncoderOptions required, EncoderOptions other, string argName)
        {
            if(!TypeEquals(required, other))
                throw new ArgumentException(
                    $"The encoder options are not of the required type '{required.GetType()}'.", argName);
        }
    }
}
