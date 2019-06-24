// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Utilities.Memory;
using System.IO;

namespace Microsoft.Xna.Framework.Content.Pipeline.Audio
{
    /// <summary>
    /// Encapsulates the native audio format (WAVEFORMATEX) information of the audio content.
    /// </summary>
    public sealed class AudioFormat
    {
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
        public int Format { get; }

        /// <summary>
        /// Gets the raw byte buffer for the format. For non-PCM formats, this buffer contains important format-specific information beyond the basic format information exposed in other properties of the AudioFormat type.
        /// </summary>
        /// <value>The raw byte buffer represented in a collection.</value>
        public byte[] NativeWaveFormat { get; }

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
            AverageBytesPerSecond = averageBytesPerSecond;
            BitsPerSample = bitsPerSample;
            BlockAlign = blockAlign;
            ChannelCount = channelCount;
            Format = format;
            SampleRate = sampleRate;

            NativeWaveFormat = ConstructNativeWaveFormat();
        }

        private byte[] ConstructNativeWaveFormat()
        {
            using (var memory = RecyclableMemoryManager.Instance.GetMemoryStream())
            using (var writer = new BinaryWriter(memory))
            {
                writer.Write((short)Format);
                writer.Write((short)ChannelCount);
                writer.Write((int)SampleRate);
                writer.Write((int)AverageBytesPerSecond);
                writer.Write((short)BlockAlign);
                writer.Write((short)BitsPerSample);
                writer.Write((short)0);

                return memory.ToArray();
            }

        }
    }
}