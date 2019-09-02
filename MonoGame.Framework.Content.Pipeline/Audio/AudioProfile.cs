﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Linq;


namespace MonoGame.Framework.Content.Pipeline.Audio
{
    public abstract class AudioProfile
    {
        private static readonly LoadedTypeCollection<AudioProfile> _profiles =
            new LoadedTypeCollection<AudioProfile>();

        /// <summary>
        /// Find the profile for this target platform.
        /// </summary>
        /// <param name="platform">The platform target for audio.</param>
        /// <returns></returns>
        public static AudioProfile ForPlatform(TargetPlatform platform)
        {
            var profile = _profiles.FirstOrDefault(h => h.Supports(platform));
            if (profile != null)
                return profile;

            throw new PipelineException(
                "There is no supported audio profile for the '" + platform + "' platform!");
        }

        /// <summary>
        /// Returns true if this profile supports audio processing for this platform.
        /// </summary>
        public abstract bool Supports(TargetPlatform platform);

        /// <summary>
        /// Converts the audio content to work on targeted platform.
        /// </summary>
        /// <param name="platform">The platform to build the audio content for.</param>
        /// <param name="quality">The suggested audio quality level.</param>
        /// <param name="content">The audio content to convert.</param>
        /// <returns>The quality used for conversion which could be different from the suggested quality.</returns>
        public abstract ConversionQuality ConvertAudio(
            TargetPlatform platform, ConversionQuality quality, AudioContent content);

        /// <summary>
        /// Converts the audio content to a streaming format that works on targeted platform.
        /// </summary>
        /// <param name="platform">The platform to build the audio content for.</param>
        /// <param name="quality">The suggested audio quality level.</param>
        /// <param name="content">he audio content to convert.</param>
        /// <param name="inputFile">The input file name. This may change varying on platform and profile.</param>
        /// <param name="outputFile">The output file name, changed by platform and profile.</param>
        /// <returns>The quality used for conversion which could be different from the suggested quality.</returns>
        public abstract ConversionQuality ConvertStreamingAudio(
            TargetPlatform platform, ConversionQuality quality, AudioContent content, 
            string inputFile, out string outputFile);

        protected static int QualityToSampleRate(
            ConversionFormat format, ConversionQuality quality, int sourceSampleRate)
        {
            switch (quality)
            {
                case ConversionQuality.Low:
                    return Math.Max(8000, (int)Math.Floor(sourceSampleRate / 2.0));

                case ConversionQuality.Medium:
                    return Math.Max(8000, (int)Math.Floor(sourceSampleRate * 3 / 4.0));

                default:
                    return Math.Max(8000, sourceSampleRate);
            }
        }

        protected static int QualityToBitRate(ConversionQuality quality)
        {
            switch (quality)
            {
                case ConversionQuality.Low:
                    return 96000;
                case ConversionQuality.Medium:
                    return 128000;
            }

            return 192000;
        }
    }
}
