using MonoGame.OpenAL;
using System;

namespace MonoGame.Framework.Audio
{
    internal static class ALHelper
    {
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHidden]
        internal static void CheckError(string message = "", params object[] args)
        {
            ALError error;
            if ((error = AL.GetError()) != ALError.NoError)
            {
                if (args != null && args.Length > 0)
                    message = string.Format(message, args);

                throw new InvalidOperationException(message + " (Reason: " + AL.GetErrorString(error) + ")");
            }
        }

        public static bool IsStereoFormat(ALFormat format)
        {
            return (format == ALFormat.Stereo8
                || format == ALFormat.Stereo16
                || format == ALFormat.StereoFloat32
                || format == ALFormat.StereoIma4
                || format == ALFormat.StereoMSAdpcm);
        }

        public static ALFormat GetALFormat(AudioChannels channels, bool isFloat)
        {
            switch (channels)
            {
                case AudioChannels.Mono:
                    return isFloat ? ALFormat.MonoFloat32 : ALFormat.Mono16;

                case AudioChannels.Stereo:
                    return isFloat ? ALFormat.StereoFloat32 : ALFormat.Stereo16;

                default:
                    throw new ArgumentOutOfRangeException(nameof(channels), "Only mono and stereo channels are supported.");
            }
        }
    }

    internal static class ALCHelper
    {
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHidden]
        internal static void CheckError(string message = "", params object[] args)
        {
            ALCError error;
            if ((error = ALC.GetError()) != ALCError.NoError)
            {
                if (args != null && args.Length > 0)
                    message = string.Format(message, args);

                throw new InvalidOperationException(message + " (Reason: " + error.ToString() + ")");
            }
        }
    }
}
