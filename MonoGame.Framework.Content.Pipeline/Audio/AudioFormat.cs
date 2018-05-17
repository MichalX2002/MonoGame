// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.Xna.Framework.Content.Pipeline.Audio
{
    /// <summary>
    /// Encapsulates the native audio format (WAVEFORMATEX) information of the audio content.
    /// </summary>
    public sealed class AudioFormat
    {
        int format;
        List<byte> nativeWaveFormat;

        /// <summary>
        /// Gets the average bytes processed per second.
        /// </summary>
        /// <value>Average bytes processed per second.</value>
        public int AverageBytesPerSecond { get; }

        /// <summary>
        /// Gets the bit depth of the audio content.
        /// </summary>
        /// <value>If the audio has not been processed, the source bit depth; otherwise, the bit depth of the new format.</value>
        public int BitsPerSample { get; }

        /// <summary>
        /// Gets the number of bytes per sample block, taking channels into consideration. For example, for 16-bit stereo audio (PCM format), the size of each sample block is 4 bytes.
        /// </summary>
        /// <value>Number of bytes, per sample block.</value>
        public int BlockAlign { get; }

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        /// <value>If the audio has not been processed, the source channel count; otherwise, the new channel count.</value>
        public int ChannelCount { get; }

        /// <summary>
        /// Gets the format of the audio content.
        /// </summary>
        /// <value>If the audio has not been processed, the format tag of the source content; otherwise, the new format tag.</value>
        public int Format { get { return format; } }

        /// <summary>
        /// Gets the raw byte buffer for the format. For non-PCM formats, this buffer contains important format-specific information beyond the basic format information exposed in other properties of the AudioFormat type.
        /// </summary>
        /// <value>The raw byte buffer represented in a collection.</value>
        public ReadOnlyCollection<byte> NativeWaveFormat { get { return nativeWaveFormat.AsReadOnly(); } }

        /// <summary>
        /// Gets the sample rate of the audio content.
        /// </summary>
        /// <value>If the audio has not been processed, the source sample rate; otherwise, the new sample rate.</value>
        public int SampleRate { get; }

        internal AudioFormat(
            int averageBytesPerSecond,
            int bitsPerSample,
            int blockAlign,
            int channelCount,
            int format,
            int sampleRate)
        {
            this.AverageBytesPerSecond = averageBytesPerSecond;
            this.BitsPerSample = bitsPerSample;
            this.BlockAlign = blockAlign;
            this.ChannelCount = channelCount;
            this.format = format;
            this.SampleRate = sampleRate;

            this.nativeWaveFormat = this.ConstructNativeWaveFormat();
        }

        private List<byte> ConstructNativeWaveFormat()
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write((short)this.format);
                    writer.Write((short)this.ChannelCount);
                    writer.Write((int)this.SampleRate);
                    writer.Write((int)this.AverageBytesPerSecond);
                    writer.Write((short)this.BlockAlign);
                    writer.Write((short)this.BitsPerSample);
                    writer.Write((short)0);

                    var bytes = new byte[memory.Position];
                    memory.Seek(0, SeekOrigin.Begin);
                    memory.Read(bytes, 0, bytes.Length);
                    return bytes.ToList();
                }
            }
        }
    }
}
