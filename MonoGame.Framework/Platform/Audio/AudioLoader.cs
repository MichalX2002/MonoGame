// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using MonoGame.Framework.IO;
using MonoGame.Framework.Memory;
using MonoGame.OpenAL;

namespace MonoGame.Framework.Audio
{
    internal static partial class AudioLoader
    {
        internal const int FormatPcm = 1;
        internal const int FormatMsAdpcm = 2;
        internal const int FormatIeee = 3;
        internal const int FormatIma4 = 17;

        [DebuggerHidden]
        private static Exception GetChannelCountNotSupported()
        {
            return new NotSupportedException("The specified channel count is not supported.");
        }

        public static ALFormat GetSoundFormat(int format, int channels, int bits)
        {
            return format switch
            {
                // PCM
                FormatPcm => channels switch
                {
                    1 => bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
                    2 => bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
                    _ => throw GetChannelCountNotSupported(),
                },

                // Microsoft ADPCM
                FormatMsAdpcm => channels switch
                {
                    1 => ALFormat.MonoMSAdpcm,
                    2 => ALFormat.StereoMSAdpcm,
                    _ => throw GetChannelCountNotSupported(),
                },

                // IEEE Float
                FormatIeee => channels switch
                {
                    1 => ALFormat.MonoFloat32,
                    2 => ALFormat.StereoFloat32,
                    _ => throw GetChannelCountNotSupported(),
                },

                // IMA4 ADPCM
                FormatIma4 => channels switch
                {
                    1 => ALFormat.MonoIma4,
                    2 => ALFormat.StereoIma4,
                    _ => throw GetChannelCountNotSupported(),
                },

                _ => throw new NotSupportedException(
                    "The specified sound format (" + format + ") is not supported."),
            };
        }

        // Converts block alignment in bytes to sample alignment, primarily for compressed formats
        // Calculation of sample alignment from http://kcat.strangesoft.net/openal-extensions/SOFT_block_alignment.txt
        public static int SampleAlignment(ALFormat format, int blockAlignment)
        {
            switch (format)
            {
                case ALFormat.MonoIma4:
                    return (blockAlignment - 4) / 4 * 8 + 1;
                case ALFormat.StereoIma4:
                    return (blockAlignment / 2 - 4) / 4 * 8 + 1;
                case ALFormat.MonoMSAdpcm:
                    return (blockAlignment - 7) * 2 + 2;
                case ALFormat.StereoMSAdpcm:
                    return (blockAlignment / 2 - 7) * 2 + 2;
            }
            return 0;
        }

        /// <summary>
        /// Decodes audio samples from a stream.
        /// </summary>
        /// <param name="stream">The encoded stream .</param>
        /// <param name="format">Gets the OpenAL format enumeration value.</param>
        /// <param name="frequency">Gets the frequency or sample rate.</param>
        /// <param name="channels">Gets the number of channels.</param>
        /// <param name="blockAlignment">Gets the block alignment, important for compressed sounds.</param>
        /// <param name="bitsPerSample">Gets the number of bits per sample.</param>
        /// <param name="samplesPerBlock">Gets the number of samples per block.</param>
        /// <param name="sampleCount">Gets the total number of samples.</param>
        /// <returns>The large buffer containing the waveform data or compressed blocks.</returns>
        public static RecyclableBuffer Load(
            Stream stream, out ALFormat format, out int frequency, out int channels,
            out int blockAlignment, out int bitsPerSample, out int samplesPerBlock, out int sampleCount)
        {
            string signature;
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                signature = new string(reader.ReadChars(4));

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                byte[] sigBytes = Encoding.UTF8.GetBytes(signature);
                stream = new PrefixedStream(sigBytes, stream, leaveOpen: false);
            }

            using (var buffered = RecyclableMemoryManager.Default.GetBufferedStream(stream, leaveOpen: false))
            {
                switch (signature)
                {
#if NVORBIS
                    case "OggS":
                        return LoadVorbis(
                            buffered, out format, out frequency, out channels, out blockAlignment,
                            out bitsPerSample, out samplesPerBlock, out sampleCount);
#endif

                    case "RIFF":
                        return LoadWave(
                            buffered, out format, out frequency, out channels, out blockAlignment,
                            out bitsPerSample, out samplesPerBlock, out sampleCount);

                    default:
                        throw new NotSupportedException($"Unknown file signature ({signature}).");
                }
            }
        }

        private static RecyclableBuffer LoadWave(
            Stream stream, out ALFormat format, out int frequency, out int channels,
            out int blockAlignment, out int bitsPerSample, out int samplesPerBlock, out int sampleCount)
        {
            using (var reader = new BinaryReader(stream))
            {
                var signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new ArgumentException("Specified stream is not a RIFF file.");
                reader.ReadInt32(); // riff_chunk_size

                var wformat = new string(reader.ReadChars(4));
                if (wformat != "WAVE")
                    throw new ArgumentException("Specified stream is not a WAVE file.");

                RecyclableBuffer? audioData = null;
                int audioFormat = 0;
                channels = 0;
                bitsPerSample = 0;
                format = ALFormat.Mono16;
                frequency = 0;
                blockAlignment = 0;
                samplesPerBlock = 0;
                sampleCount = 0;

                try
                {
                    // WAVE header
                    while (audioData == null)
                    {
                        string chunkType = new string(reader.ReadChars(4));
                        int chunkSize = reader.ReadInt32();
                        switch (chunkType)
                        {
                            case "fmt ":
                            {
                                audioFormat = reader.ReadInt16();    // 2
                                channels = reader.ReadInt16();       // 4
                                frequency = reader.ReadInt32();      // 8
                                int byteRate = reader.ReadInt32();   // 12
                                blockAlignment = reader.ReadInt16(); // 14
                                bitsPerSample = reader.ReadInt16();  // 16

                                // Read extra data if present
                                if (chunkSize > 16)
                                {
                                    int extraDataSize = reader.ReadInt16();
                                    if (audioFormat == FormatIma4)
                                    {
                                        samplesPerBlock = reader.ReadInt16();
                                        extraDataSize -= 2;
                                    }
                                    if (extraDataSize > 0)
                                    {
                                        if (reader.BaseStream.CanSeek)
                                            reader.BaseStream.Seek(extraDataSize, SeekOrigin.Current);
                                        else
                                        {
                                            for (int i = 0; i < extraDataSize; i++)
                                                reader.ReadByte();
                                        }
                                    }
                                }
                            }
                            break;

                            case "fact":
                                if (audioFormat == FormatIma4)
                                {
                                    sampleCount = reader.ReadInt32() * channels;
                                    chunkSize -= 4;
                                }
                                // Skip any remaining chunk data
                                if (chunkSize > 0)
                                {
                                    if (reader.BaseStream.CanSeek)
                                        reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                                    else
                                    {
                                        for (int i = 0; i < chunkSize; i++)
                                            reader.ReadByte();
                                    }
                                }
                                break;

                            case "data":
                                audioData = RecyclableBuffer.ReadBytes(
                                    reader.BaseStream, chunkSize, nameof(audioData));
                                break;

                            default:
                                // Skip this chunk
                                if (reader.BaseStream.CanSeek)
                                    reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                                else
                                {
                                    for (int i = 0; i < chunkSize; i++)
                                        reader.ReadByte();
                                }
                                break;
                        }
                    }

                    // Calculate fields we didn't read from the file
                    format = GetSoundFormat(audioFormat, channels, bitsPerSample);

                    if (samplesPerBlock == 0)
                        samplesPerBlock = SampleAlignment(format, blockAlignment);

                    if (sampleCount == 0)
                    {
                        switch (audioFormat)
                        {
                            case FormatIma4:
                            case FormatMsAdpcm:
                                sampleCount =
                                    (audioData.BaseLength / blockAlignment * samplesPerBlock) +
                                    SampleAlignment(format, audioData.BaseLength % blockAlignment);
                                break;

                            case FormatPcm:
                            case FormatIeee:
                                sampleCount = audioData.BaseLength / (channels * bitsPerSample / 8);
                                break;

                            default:
                                throw new InvalidDataException("Unhandled WAV format " + format.ToString());
                        }
                    }

                    return audioData;
                }
                catch
                {
                    audioData?.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// Convert buffer containing 24-bit signed PCM wav data to a 16-bit signed PCM buffer
        /// </summary>
        internal static unsafe byte[] Convert24To16(
            ReadOnlySpan<byte> data, out string? bufferTag, out int size)
        {
            if (data.Length % 3 != 0)
                throw new ArgumentException("Invalid 24-bit PCM data received");

            // Sample count includes both channels if stereo
            int sampleCount = data.Length / 3;
            size = sampleCount * sizeof(short);
            bool isRecyclable = size <= RecyclableMemoryManager.Default.MaximumBufferSize;

            bufferTag = isRecyclable ? nameof(Convert24To16) : null;
            var outData = isRecyclable
                ? RecyclableMemoryManager.Default.GetBuffer(size, bufferTag).Buffer
                : new byte[size];

            fixed (byte* src = data)
            fixed (byte* dst = outData)
            {
                int srcIndex = 0;
                int dstIndex = 0;
                for (int i = 0; i < sampleCount; i++)
                {
                    // TODO: SIMD shuffle?

                    // Drop the least significant byte from the 24-bit sample to get the 16-bit sample
                    dst[dstIndex] = src[srcIndex + 1];
                    dst[dstIndex + 1] = src[srcIndex + 2];
                    dstIndex += 2;
                    srcIndex += 3;
                }
            }
            return outData;
        }

        /// <summary>
        /// Convert buffer containing IEEE 32-bit float data to a 16-bit signed PCM buffer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static void ConvertSingleToInt16(ReadOnlySpan<float> src, Span<short> dst)
        {
            if (dst.Length < src.Length)
                throw new ArgumentException("The destination span is too small.");

            // TODO: remove ANDROID conditional in NET5
#if !ANDROID
            if (Vector.IsHardwareAccelerated)
            {
                var vMax = new Vector<float>(short.MaxValue);
                var vMin = new Vector<float>(short.MinValue);
                var vHalf = new Vector<float>(0.5f);

                while (src.Length >= Vector<float>.Count * 2)
                {
                    var vSrc1 = new Vector<float>(src);
                    src = src[Vector<float>.Count..];
                    var vResult1 = Vector.Add(Vector.Multiply(vSrc1, vMax), vHalf);
                    vResult1 = Vector.Max(vMin, Vector.Min(vResult1, vMax));
                    var vIntResult1 = Vector.ConvertToInt32(vResult1);

                    var vSrc2 = new Vector<float>(src);
                    src = src[Vector<float>.Count..];
                    var vResult2 = Vector.Add(Vector.Multiply(vSrc2, vMax), vHalf);
                    vResult2 = Vector.Max(vMin, Vector.Min(vResult2, vMax));
                    var vIntResult2 = Vector.ConvertToInt32(vResult2);

                    var vResult = Vector.Narrow(vIntResult1, vIntResult2);
                    vResult.CopyTo(dst);
                    dst = dst[Vector<short>.Count..];
                }
            }
#endif

            for (int i = 0; i < src.Length; i++)
            {
                int tmp = (int)(src[i] * short.MaxValue);
                if (tmp > short.MaxValue)
                    dst[i] = short.MaxValue;
                else if (tmp < short.MinValue)
                    dst[i] = short.MinValue;
                else
                    dst[i] = (short)tmp;
            }
        }

        // TODO: validate IMA4
        #region IMA4 decoding

        // Step table
        private static int[] StepTable { get; } = new int[]
        {
            7, 8, 9, 10, 11, 12, 13, 14,
            16, 17, 19, 21, 23, 25, 28, 31,
            34, 37, 41, 45, 50, 55, 60, 66,
            73, 80, 88, 97, 107, 118, 130, 143,
            157, 173, 190, 209, 230, 253, 279, 307,
            337, 371, 408, 449, 494, 544, 598, 658,
            724, 796, 876, 963, 1060, 1166, 1282, 1411,
            1552, 1707, 1878, 2066, 2272, 2499, 2749, 3024,
            3327, 3660, 4026, 4428, 4871, 5358, 5894, 6484,
            7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
            15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794,
            32767
        };

        // Does not allocate by returning reference to assembly.
        private static ReadOnlySpan<sbyte> IndexTable => new sbyte[]
        {
            // ADPCM data size is 4
            -1, -1, -1, -1, 2, 4, 6, 8,
            -1, -1, -1, -1, 2, 4, 6, 8
        };

        private struct ImaState
        {
            public int Predictor;
            public int StepIndex;
        }

        private static short AdpcmImaWavExpandNibble(ref ImaState channel, int nibble)
        {
            int diff = StepTable[channel.StepIndex] >> 3;
            if ((nibble & 0x04) != 0)
                diff += StepTable[channel.StepIndex];
            if ((nibble & 0x02) != 0)
                diff += StepTable[channel.StepIndex] >> 1;
            if ((nibble & 0x01) != 0)
                diff += StepTable[channel.StepIndex] >> 2;

            if ((nibble & 0x08) != 0)
                channel.Predictor -= diff;
            else
                channel.Predictor += diff;

            channel.StepIndex = MathHelper.Clamp(channel.StepIndex + IndexTable[nibble], 0, 88);

            return MathHelper.Clamp(channel.Predictor, short.MinValue, short.MaxValue);
        }

        /// <summary>
        /// Convert buffer containing IMA/ADPCM data to a 16-bit signed PCM buffer.
        /// </summary>
        internal static short[] ConvertIma4ToPcm(ReadOnlySpan<byte> source, int channels, int blockAlignment)
        {
            int count = source.Length;

            var channel0 = new ImaState();
            var channel1 = new ImaState();

            int sampleCountFullBlock = ((blockAlignment / channels) - 4) / 4 * 8 + 1;
            int sampleCountLastBlock = 0;
            if (count % blockAlignment > 0)
                sampleCountLastBlock = ((count % blockAlignment / channels) - 4) / 4 * 8 + 1;
            int sampleCount = (count / blockAlignment * sampleCountFullBlock) + sampleCountLastBlock;
            var samples = new short[sampleCount * channels];
            var sampleDst = samples.AsSpan();

            int offset = 0;
            while (count > 0)
            {
                int blockSize = blockAlignment;
                if (count < blockSize)
                    blockSize = count;
                count -= blockAlignment;

                channel0.Predictor = source[offset++];
                channel0.Predictor |= source[offset++] << 8;
                if ((channel0.Predictor & 0x8000) != 0)
                    channel0.Predictor -= 0x10000;
                channel0.StepIndex = source[offset++];
                if (channel0.StepIndex > 88)
                    channel0.StepIndex = 88;
                offset++;

                sampleDst[0] = (short)channel0.Predictor;
                sampleDst = sampleDst[1..];

                if (channels == 2)
                {
                    channel1.Predictor = source[offset++];
                    channel1.Predictor |= source[offset++] << 8;
                    if ((channel1.Predictor & 0x8000) != 0)
                        channel1.Predictor -= 0x10000;
                    channel1.StepIndex = source[offset++];
                    if (channel1.StepIndex > 88)
                        channel1.StepIndex = 88;
                    offset++;

                    sampleDst[0] = (short)channel1.Predictor;
                    sampleDst = sampleDst[1..];

                    for (int nibbles = 2 * (blockSize - 8); nibbles > 0; nibbles -= 16)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            byte nibble = source[offset + i];
                            short sample1 = AdpcmImaWavExpandNibble(ref channel0, nibble & 0x0f);
                            short sample2 = AdpcmImaWavExpandNibble(ref channel0, nibble >> 4);

                            int index = i * 4;
                            sampleDst[index + 2] = sample2;
                            sampleDst[index + 0] = sample1;
                        }
                        offset += 4;

                        for (int i = 0; i < 4; i++)
                        {
                            byte nibble = source[offset + i];
                            short sample1 = AdpcmImaWavExpandNibble(ref channel1, nibble & 0x0f);
                            short sample2 = AdpcmImaWavExpandNibble(ref channel1, nibble >> 4);

                            int index = i * 4 + 1;
                            sampleDst[index + 2] = sample2;
                            sampleDst[index + 0] = sample1;
                        }
                        offset += 4;

                        sampleDst = sampleDst[16..];
                    }
                }
                else
                {
                    for (int nibbles = 2 * (blockSize - 4); nibbles > 0; nibbles -= 2)
                    {
                        byte nibble = source[offset];
                        short sample1 = AdpcmImaWavExpandNibble(ref channel0, nibble & 0x0f);
                        short sample2 = AdpcmImaWavExpandNibble(ref channel0, nibble >> 4);

                        sampleDst[1] = sample2;
                        sampleDst[0] = sample1;
                        sampleDst = sampleDst[2..];

                        offset++;
                    }
                }
            }

            return samples;
        }

        #endregion

        // TODO: validate MS-ADPCM
        #region MS-ADPCM decoding

        private static int[] AdaptationTable { get; } = new int[]
        {
            230, 230, 230, 230, 307, 409, 512, 614,
            768, 614, 512, 409, 307, 230, 230, 230
        };

        private static int[] AdaptationCoeff1 { get; } = new int[]
        {
            256, 512, 0, 192, 240, 460, 392
        };

        private static int[] AdaptationCoeff2 { get; } = new int[]
        {
            0, -256, 0, 64, 0, -208, -232
        };

        private struct MsAdpcmState
        {
            public int Delta;
            public int Sample1;
            public int Sample2;
            public int Coeff1;
            public int Coeff2;
        }

        private static short AdpcmMsExpandNibble(ref MsAdpcmState channel, int nibble)
        {
            int nibbleSign = nibble - (((nibble & 0x08) != 0) ? 0x10 : 0);
            int predictor =
                ((channel.Sample1 * channel.Coeff1) +
                (channel.Sample2 * channel.Coeff2)) / 256 +
                (nibbleSign * channel.Delta);

            channel.Sample2 = channel.Sample1;
            channel.Sample1 = predictor;

            channel.Delta = AdaptationTable[nibble] * channel.Delta / 256;
            if (channel.Delta < 16)
                channel.Delta = 16;

            return MathHelper.Clamp(predictor, short.MinValue, short.MaxValue);
        }

        /// <summary>
        /// Convert buffer containing MS-ADPCM data to a 16-bit signed PCM buffer.
        /// </summary>
        internal static short[] ConvertMsAdpcmToPcm(ReadOnlySpan<byte> source, int channels, int blockAlignment)
        {
            int count = source.Length;
            bool stereo = channels == 2;

            var channel0 = new MsAdpcmState();
            var channel1 = new MsAdpcmState();
            int blockPredictor;

            int sampleCountFullBlock = ((blockAlignment / channels) - 7) * 2 + 2;
            int sampleCountLastBlock = 0;
            if (count % blockAlignment > 0)
                sampleCountLastBlock = ((count % blockAlignment / channels) - 7) * 2 + 2;
            int sampleCount = (count / blockAlignment * sampleCountFullBlock) + sampleCountLastBlock;
            var samples = new short[sampleCount * channels];
            var sampleDst = samples.AsSpan();

            int offset = 0;
            while (count > 0)
            {
                int blockSize = blockAlignment;
                if (count < blockSize)
                    blockSize = count;
                count -= blockAlignment;

                int totalSamples = ((blockSize / channels) - 7) * 2 + 2;
                if (totalSamples < 2)
                    break;

                int offsetStart = offset;
                blockPredictor = source[offset];
                ++offset;
                if (blockPredictor > 6)
                    blockPredictor = 6;
                channel0.Coeff1 = AdaptationCoeff1[blockPredictor];
                channel0.Coeff2 = AdaptationCoeff2[blockPredictor];
                if (stereo)
                {
                    blockPredictor = source[offset];
                    ++offset;
                    if (blockPredictor > 6)
                        blockPredictor = 6;
                    channel1.Coeff1 = AdaptationCoeff1[blockPredictor];
                    channel1.Coeff2 = AdaptationCoeff2[blockPredictor];
                }

                channel0.Delta = source[offset];
                channel0.Delta |= source[offset + 1] << 8;
                if ((channel0.Delta & 0x8000) != 0)
                    channel0.Delta -= 0x10000;
                offset += 2;
                if (stereo)
                {
                    channel1.Delta = source[offset];
                    channel1.Delta |= source[offset + 1] << 8;
                    if ((channel1.Delta & 0x8000) != 0)
                        channel1.Delta -= 0x10000;
                    offset += 2;
                }

                channel0.Sample1 = source[offset];
                channel0.Sample1 |= source[offset + 1] << 8;
                if ((channel0.Sample1 & 0x8000) != 0)
                    channel0.Sample1 -= 0x10000;
                offset += 2;
                if (stereo)
                {
                    channel1.Sample1 = source[offset];
                    channel1.Sample1 |= source[offset + 1] << 8;
                    if ((channel1.Sample1 & 0x8000) != 0)
                        channel1.Sample1 -= 0x10000;
                    offset += 2;
                }

                channel0.Sample2 = source[offset];
                channel0.Sample2 |= source[offset + 1] << 8;
                if ((channel0.Sample2 & 0x8000) != 0)
                    channel0.Sample2 -= 0x10000;
                offset += 2;
                if (stereo)
                {
                    channel1.Sample2 = source[offset];
                    channel1.Sample2 |= source[offset + 1] << 8;
                    if ((channel1.Sample2 & 0x8000) != 0)
                        channel1.Sample2 -= 0x10000;
                    offset += 2;
                }

                if (stereo)
                {
                    sampleDst[3] = (short)channel1.Sample1;
                    sampleDst[2] = (short)channel0.Sample1;
                    sampleDst[1] = (short)channel1.Sample2;
                    sampleDst[0] = (short)channel0.Sample2;
                    sampleDst = sampleDst[4..];
                }
                else
                {
                    sampleDst[1] = (short)channel0.Sample1;
                    sampleDst[0] = (short)channel0.Sample2;
                    sampleDst = sampleDst[2..];
                }

                blockSize -= offset - offsetStart;

                var srcSlice = source.Slice(offset, blockSize);
                offset += srcSlice.Length;

                if (stereo)
                {
                    for (int i = 0; i < srcSlice.Length; i++)
                    {
                        byte nibbles = srcSlice[i];
                        short sample1 = AdpcmMsExpandNibble(ref channel0, nibbles >> 4);
                        short sample2 = AdpcmMsExpandNibble(ref channel1, nibbles & 0x0f);

                        sampleDst[1] = sample2;
                        sampleDst[0] = sample1;
                        sampleDst = sampleDst[2..];
                    }
                }
                else
                {
                    for (int i = 0; i < srcSlice.Length; i++)
                    {
                        byte nibbles = srcSlice[offset];
                        short sample1 = AdpcmMsExpandNibble(ref channel0, nibbles >> 4);
                        short sample2 = AdpcmMsExpandNibble(ref channel0, nibbles & 0x0f);

                        sampleDst[1] = sample2;
                        sampleDst[0] = sample1;
                        sampleDst = sampleDst[2..];
                    }
                }
            }
            return samples;
        }

        #endregion
    }
}
