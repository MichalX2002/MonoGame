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
            return format == ALFormat.Stereo8
                || format == ALFormat.Stereo16
                || format == ALFormat.StereoFloat32
                || format == ALFormat.StereoIma4
                || format == ALFormat.StereoMSAdpcm;
        }

        public static ALFormat GetALFormat(AudioChannels channels, AudioDepth depth)
        {
            switch (channels)
            {
                case AudioChannels.Mono:
                    switch (depth)
                    {
                        case AudioDepth.Short: return ALFormat.Mono16;
                        case AudioDepth.Float: return ALFormat.MonoFloat32;
                    }
                    break;

                case AudioChannels.Stereo:
                    switch (depth)
                    {
                        case AudioDepth.Short: return ALFormat.Stereo16;
                        case AudioDepth.Float: return ALFormat.StereoFloat32;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(channels), "Only mono and stereo channels are supported.");
            }
            throw new ArgumentOutOfRangeException(nameof(depth), "Audio format is not supported.");
        }

        public static ALFormat GetALFormat(AudioChannels channels, bool isFloat)
        {
            return GetALFormat(channels, isFloat ? AudioDepth.Float : AudioDepth.Short);
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
