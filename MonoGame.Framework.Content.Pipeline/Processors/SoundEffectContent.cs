﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Represents a processed sound effect.
    /// </summary>
    [CompressedContent]
    public sealed class SoundEffectContent
    {
        internal byte[] format;
        internal Stream data;
        internal int dataLength;
        internal int loopStart;
        internal int loopLength;
        internal TimeSpan duration;

        /// <summary>
        /// Initializes a new instance of the SoundEffectContent class.
        /// </summary>
        /// <param name="format">The WAV header.</param>
        /// <param name="data">The audio waveform data stream.</param>
        /// <param name="dataLength">The amount to read from data.</param>
        /// <param name="loopStart">The start of the loop segment (must be block aligned).</param>
        /// <param name="loopLength">The length of the loop segment (must be block aligned).</param>
        /// <param name="duration">The duration of the sound file.</param>
        internal SoundEffectContent(
            byte[] format, Stream data, int dataLength, int loopStart, int loopLength, TimeSpan duration)
        {
            this.format = format;
            this.data = data;
            this.dataLength = dataLength;
            this.loopStart = loopStart;
            this.loopLength = loopLength;
            this.duration = duration;
        }
    }
}
