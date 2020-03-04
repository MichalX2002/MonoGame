// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
        private static Exception GetChannelCountNotSupportedException()
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
                    _ => throw GetChannelCountNotSupportedException(),
                },

                // Microsoft ADPCM
                FormatMsAdpcm => channels switch
                {
                    1 => ALFormat.MonoMSAdpcm,
                    2 => ALFormat.StereoMSAdpcm,
                    _ => throw GetChannelCountNotSupportedException(),
                },

                // IEEE Float
                FormatIeee => channels switch
                {
                    1 => ALFormat.MonoFloat32,
                    2 => ALFormat.StereoFloat32,
                    _ => throw GetChannelCountNotSupportedException(),
                },

                // IMA4 ADPCM
                FormatIma4 => channels switch
                {
                    1 => ALFormat.MonoIma4,
                    2 => ALFormat.StereoIma4,
                    _ => throw GetChannelCountNotSupportedException(),
                },

                _ => throw new NotSupportedException("The specified sound format (" + format.ToString() + ") is not supported."),
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
        /// <returns>The stream containing the waveform data or compressed blocks.</returns>
        public static RecyclableMemoryStream Load(
            Stream stream, out ALFormat format, out int frequency, out int channels,
            out int blockAlignment, out int bitsPerSample, out int samplesPerBlock, out int sampleCount)
        {
            string signature;
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                signature = new string(reader.ReadChars(4));

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            else
            {
                byte[] sigBytes = Encoding.UTF8.GetBytes(signature);
                stream = new PrefixedStream(sigBytes, stream, leaveOpen: false);
            }

            using (var buffered = RecyclableMemoryManager.Default.GetBufferedStream(stream, false))
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

        public static RecyclableMemoryStream ReadBytes(Stream stream, int bytes)
        {
            var result = RecyclableMemoryManager.Default.GetMemoryStream(nameof(ReadBytes), bytes);
            byte[] buffer = RecyclableMemoryManager.Default.GetBlock();
            try
            {
                int left = bytes;
                do
                {
                    int toRead = Math.Min(left, buffer.Length);
                    int n = stream.Read(buffer, 0, toRead);
                    if (n == 0)
                        break;
                    result.Write(buffer, 0, n);
                    left -= n;
                } while (left > 0);

                if (left > 0)
                    throw new EndOfStreamException("Failed to read enough data.");

                result.Position = 0;
                return result;
            }
            catch
            {
                result.Dispose();
                throw;
            }
            finally
            {
                RecyclableMemoryManager.Default.ReturnBlock(buffer);
            }
        }

        private static RecyclableMemoryStream LoadWave(
            Stream stream, out ALFormat format, out int frequency, out int channels,
            out int blockAlignment, out int bitsPerSample, out int samplesPerBlock, out int sampleCount)
        {
            using (var reader = new BinaryReader(stream))
            {
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new ArgumentException("Specified stream is not a wave file.");
                reader.ReadInt32(); // riff_chunk_size

                string wformat = new string(reader.ReadChars(4));
                if (wformat != "WAVE")
                    throw new ArgumentException("Specified stream is not a wave file.");

                RecyclableMemoryStream audioData = null;
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
                                audioFormat = reader.ReadInt16(); // 2
                                channels = reader.ReadInt16(); // 4
                                frequency = reader.ReadInt32();  // 8
                                int byteRate = reader.ReadInt32();    // 12
                                blockAlignment = reader.ReadInt16();  // 14
                                bitsPerSample = reader.ReadInt16(); // 16

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
                                            for (int i = 0; i < extraDataSize; ++i)
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
                                        for (int i = 0; i < chunkSize; ++i)
                                            reader.ReadByte();
                                    }
                                }
                                break;

                            case "data":
                                audioData = ReadBytes(reader.BaseStream, chunkSize);
                                break;

                            default:
                                // Skip this chunk
                                if (reader.BaseStream.CanSeek)
                                    reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                                else
                                {
                                    for (int i = 0; i < chunkSize; ++i)
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
                                    ((int)audioData.Length / blockAlignment * samplesPerBlock) +
                                    SampleAlignment(format, (int)audioData.Length % blockAlignment);
                                break;

                            case FormatPcm:
                            case FormatIeee:
                                sampleCount = (int)audioData.Length / (channels * bitsPerSample / 8);
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
        internal static unsafe byte[] Convert24To16<T>(
            ReadOnlySpan<T> span, out string bufferTag, out int size)
            where T : unmanaged
        {
            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(span);
            if (byteSpan.Length % 3 != 0)
                throw new ArgumentException("Invalid 24-bit PCM data received");

            // Sample count includes both channels if stereo
            int sampleCount = byteSpan.Length / 3;
            size = sampleCount * sizeof(short);
            bool isRecyclable = size <= RecyclableMemoryManager.Default.MaximumLargeBufferSize;

            bufferTag = isRecyclable ? nameof(Convert24To16) : null;
            byte[] outData = isRecyclable ?
                RecyclableMemoryManager.Default.GetLargeBuffer(size, bufferTag) : new byte[size];

            fixed (byte* src = byteSpan)
            {
                fixed (byte* dst = &outData[0])
                {
                    int srcIndex = 0;
                    int dstIndex = 0;
                    for (int i = 0; i < sampleCount; ++i)
                    {
                        // Drop the least significant byte from the 24-bit sample to get the 16-bit sample
                        dst[dstIndex] = src[srcIndex + 1];
                        dst[dstIndex + 1] = src[srcIndex + 2];
                        dstIndex += 2;
                        srcIndex += 3;
                    }
                }
            }
            return outData;
        }

        // Convert buffer containing IEEE 32-bit float wav data to a 16-bit signed PCM buffer
        internal static unsafe byte[] ConvertSingleToInt16<T>(ReadOnlySpan<T> span) where T : unmanaged
        {
            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(span);
            if (byteSpan.Length % 4 != 0)
                throw new ArgumentException("Invalid 32-bit float PCM data received.");

            // Sample count includes both channels if stereo
            int sampleCount = byteSpan.Length / 4;
            var outData = new byte[sampleCount * sizeof(short)];
            fixed (byte* src = byteSpan)
            {
                float* f = (float*)src;
                fixed (byte* dst = &outData[0])
                {
                    byte* d = dst;
                    for (int i = 0; i < sampleCount; ++i)
                    {
                        short s = (short)(*f * 32767.0f);
                        *d++ = (byte)(s & 0xff);
                        *d++ = (byte)(s >> 8);
                        f++;
                    }
                }
            }
            return outData;
        }

        public static void ConvertSingleToInt16(ReadOnlySpan<float> src, Span<short> dst)
        {
            if (dst.Length < src.Length)
                throw new ArgumentException("The destination span is too small.");

            for (int i = 0; i < src.Length; i++)
            {
                int tmp = (int)(32767f * src[i]);
                if (tmp > short.MaxValue)
                    dst[i] = short.MaxValue;
                else if (tmp < short.MinValue)
                    dst[i] = short.MinValue;
                else
                    dst[i] = (short)tmp;
            }
        }

        #region IMA4 decoding

        // Step table
        private static int[] stepTable = new int[]
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

        // Step index tables
        private static int[] indexTable = new int[]
        {
            // ADPCM data size is 4
            -1, -1, -1, -1, 2, 4, 6, 8,
            -1, -1, -1, -1, 2, 4, 6, 8
        };

        private struct ImaState
        {
            public int predictor;
            public int stepIndex;
        }

        private static int AdpcmImaWavExpandNibble(ref ImaState channel, int nibble)
        {
            int diff = stepTable[channel.stepIndex] >> 3;
            if ((nibble & 0x04) != 0)
                diff += stepTable[channel.stepIndex];
            if ((nibble & 0x02) != 0)
                diff += stepTable[channel.stepIndex] >> 1;
            if ((nibble & 0x01) != 0)
                diff += stepTable[channel.stepIndex] >> 2;
            if ((nibble & 0x08) != 0)
                channel.predictor -= diff;
            else
                channel.predictor += diff;

            if (channel.predictor < -32768)
                channel.predictor = -32768;
            else if (channel.predictor > 32767)
                channel.predictor = 32767;

            channel.stepIndex += indexTable[nibble];

            if (channel.stepIndex < 0)
                channel.stepIndex = 0;
            else if (channel.stepIndex > 88)
                channel.stepIndex = 88;

            return channel.predictor;
        }

        // Convert buffer containing IMA/ADPCM wav data to a 16-bit signed PCM buffer
        internal static byte[] ConvertIma4ToPcm<T>(ReadOnlySpan<T> span, int channels, int blockAlignment)
            where T : unmanaged
        {
            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(span);
            int count = byteSpan.Length;

            var channel0 = new ImaState();
            var channel1 = new ImaState();

            int sampleCountFullBlock = ((blockAlignment / channels) - 4) / 4 * 8 + 1;
            int sampleCountLastBlock = 0;
            if (count % blockAlignment > 0)
                sampleCountLastBlock = ((count % blockAlignment / channels) - 4) / 4 * 8 + 1;
            int sampleCount = (count / blockAlignment * sampleCountFullBlock) + sampleCountLastBlock;
            var samples = new byte[sampleCount * sizeof(short) * channels];
            int sampleOffset = 0;

            int offset = 0;
            while (count > 0)
            {
                int blockSize = blockAlignment;
                if (count < blockSize)
                    blockSize = count;
                count -= blockAlignment;

                channel0.predictor = byteSpan[offset++];
                channel0.predictor |= byteSpan[offset++] << 8;
                if ((channel0.predictor & 0x8000) != 0)
                    channel0.predictor -= 0x10000;
                channel0.stepIndex = byteSpan[offset++];
                if (channel0.stepIndex > 88)
                    channel0.stepIndex = 88;
                offset++;
                int index = sampleOffset * 2;
                samples[index] = (byte)channel0.predictor;
                samples[index + 1] = (byte)(channel0.predictor >> 8);
                ++sampleOffset;

                if (channels == 2)
                {
                    channel1.predictor = byteSpan[offset++];
                    channel1.predictor |= byteSpan[offset++] << 8;
                    if ((channel1.predictor & 0x8000) != 0)
                        channel1.predictor -= 0x10000;
                    channel1.stepIndex = byteSpan[offset++];
                    if (channel1.stepIndex > 88)
                        channel1.stepIndex = 88;
                    offset++;
                    index = sampleOffset * 2;
                    samples[index] = (byte)channel1.predictor;
                    samples[index + 1] = (byte)(channel1.predictor >> 8);
                    ++sampleOffset;
                }

                if (channels == 2)
                {
                    for (int nibbles = 2 * (blockSize - 8); nibbles > 0; nibbles -= 16)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            index = (sampleOffset + i * 4) * 2;
                            int sample = AdpcmImaWavExpandNibble(ref channel0, byteSpan[offset + i] & 0x0f);
                            samples[index] = (byte)sample;
                            samples[index + 1] = (byte)(sample >> 8);

                            index = (sampleOffset + i * 4 + 2) * 2;
                            sample = AdpcmImaWavExpandNibble(ref channel0, byteSpan[offset + i] >> 4);
                            samples[index] = (byte)sample;
                            samples[index + 1] = (byte)(sample >> 8);
                        }
                        offset += 4;

                        for (int i = 0; i < 4; i++)
                        {
                            index = (sampleOffset + i * 4 + 1) * 2;
                            int sample = AdpcmImaWavExpandNibble(ref channel1, byteSpan[offset + i] & 0x0f);
                            samples[index] = (byte)sample;
                            samples[index + 1] = (byte)(sample >> 8);

                            index = (sampleOffset + i * 4 + 3) * 2;
                            sample = AdpcmImaWavExpandNibble(ref channel1, byteSpan[offset + i] >> 4);
                            samples[index] = (byte)sample;
                            samples[index + 1] = (byte)(sample >> 8);
                        }
                        offset += 4;
                        sampleOffset += 16;
                    }
                }
                else
                {
                    for (int nibbles = 2 * (blockSize - 4); nibbles > 0; nibbles -= 2)
                    {
                        index = sampleOffset * 2;
                        int b = byteSpan[offset];
                        int sample = AdpcmImaWavExpandNibble(ref channel0, b & 0x0f);
                        samples[index] = (byte)sample;
                        samples[index + 1] = (byte)(sample >> 8);
                        index += 2;
                        sample = AdpcmImaWavExpandNibble(ref channel0, b >> 4);
                        samples[index] = (byte)sample;
                        samples[index + 1] = (byte)(sample >> 8);

                        sampleOffset += 2;
                        ++offset;
                    }
                }
            }

            return samples;
        }

        #endregion

        #region MS-ADPCM decoding

        private static int[] adaptationTable = new int[]
        {
            230, 230, 230, 230, 307, 409, 512, 614,
            768, 614, 512, 409, 307, 230, 230, 230
        };
        private static int[] adaptationCoeff1 = new int[]
        {
            256, 512, 0, 192, 240, 460, 392
        };
        private static int[] adaptationCoeff2 = new int[]
        {
            0, -256, 0, 64, 0, -208, -232
        };

        private struct MsAdpcmState
        {
            public int delta;
            public int sample1;
            public int sample2;
            public int coeff1;
            public int coeff2;
        }

        private static int AdpcmMsExpandNibble(ref MsAdpcmState channel, int nibble)
        {
            int nibbleSign = nibble - (((nibble & 0x08) != 0) ? 0x10 : 0);
            int predictor = ((channel.sample1 * channel.coeff1) + (channel.sample2 * channel.coeff2)) / 256 + (nibbleSign * channel.delta);

            if (predictor < -32768)
                predictor = -32768;
            else if (predictor > 32767)
                predictor = 32767;

            channel.sample2 = channel.sample1;
            channel.sample1 = predictor;

            channel.delta = adaptationTable[nibble] * channel.delta / 256;
            if (channel.delta < 16)
                channel.delta = 16;

            return predictor;
        }

        // Convert buffer containing MS-ADPCM wav data to a 16-bit signed PCM buffer
        internal static byte[] ConvertMsAdpcmToPcm<T>(ReadOnlySpan<T> span, int channels, int blockAlignment)
            where T : unmanaged
        {
            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(span);
            int count = byteSpan.Length;
            bool stereo = channels == 2;

            var channel0 = new MsAdpcmState();
            var channel1 = new MsAdpcmState();
            int blockPredictor;

            int sampleCountFullBlock = ((blockAlignment / channels) - 7) * 2 + 2;
            int sampleCountLastBlock = 0;
            if (count % blockAlignment > 0)
                sampleCountLastBlock = ((count % blockAlignment / channels) - 7) * 2 + 2;
            int sampleCount = (count / blockAlignment * sampleCountFullBlock) + sampleCountLastBlock;
            var samples = new byte[sampleCount * sizeof(short) * channels];
            int sampleOffset = 0;

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
                blockPredictor = byteSpan[offset];
                ++offset;
                if (blockPredictor > 6)
                    blockPredictor = 6;
                channel0.coeff1 = adaptationCoeff1[blockPredictor];
                channel0.coeff2 = adaptationCoeff2[blockPredictor];
                if (stereo)
                {
                    blockPredictor = byteSpan[offset];
                    ++offset;
                    if (blockPredictor > 6)
                        blockPredictor = 6;
                    channel1.coeff1 = adaptationCoeff1[blockPredictor];
                    channel1.coeff2 = adaptationCoeff2[blockPredictor];
                }

                channel0.delta = byteSpan[offset];
                channel0.delta |= byteSpan[offset + 1] << 8;
                if ((channel0.delta & 0x8000) != 0)
                    channel0.delta -= 0x10000;
                offset += 2;
                if (stereo)
                {
                    channel1.delta = byteSpan[offset];
                    channel1.delta |= byteSpan[offset + 1] << 8;
                    if ((channel1.delta & 0x8000) != 0)
                        channel1.delta -= 0x10000;
                    offset += 2;
                }

                channel0.sample1 = byteSpan[offset];
                channel0.sample1 |= byteSpan[offset + 1] << 8;
                if ((channel0.sample1 & 0x8000) != 0)
                    channel0.sample1 -= 0x10000;
                offset += 2;
                if (stereo)
                {
                    channel1.sample1 = byteSpan[offset];
                    channel1.sample1 |= byteSpan[offset + 1] << 8;
                    if ((channel1.sample1 & 0x8000) != 0)
                        channel1.sample1 -= 0x10000;
                    offset += 2;
                }

                channel0.sample2 = byteSpan[offset];
                channel0.sample2 |= byteSpan[offset + 1] << 8;
                if ((channel0.sample2 & 0x8000) != 0)
                    channel0.sample2 -= 0x10000;
                offset += 2;
                if (stereo)
                {
                    channel1.sample2 = byteSpan[offset];
                    channel1.sample2 |= byteSpan[offset + 1] << 8;
                    if ((channel1.sample2 & 0x8000) != 0)
                        channel1.sample2 -= 0x10000;
                    offset += 2;
                }

                if (stereo)
                {
                    samples[sampleOffset] = (byte)channel0.sample2;
                    samples[sampleOffset + 1] = (byte)(channel0.sample2 >> 8);
                    samples[sampleOffset + 2] = (byte)channel1.sample2;
                    samples[sampleOffset + 3] = (byte)(channel1.sample2 >> 8);
                    samples[sampleOffset + 4] = (byte)channel0.sample1;
                    samples[sampleOffset + 5] = (byte)(channel0.sample1 >> 8);
                    samples[sampleOffset + 6] = (byte)channel1.sample1;
                    samples[sampleOffset + 7] = (byte)(channel1.sample1 >> 8);
                    sampleOffset += 8;
                }
                else
                {
                    samples[sampleOffset] = (byte)channel0.sample2;
                    samples[sampleOffset + 1] = (byte)(channel0.sample2 >> 8);
                    samples[sampleOffset + 2] = (byte)channel0.sample1;
                    samples[sampleOffset + 3] = (byte)(channel0.sample1 >> 8);
                    sampleOffset += 4;
                }

                blockSize -= offset - offsetStart;
                if (stereo)
                {
                    for (int i = 0; i < blockSize; ++i)
                    {
                        int nibbles = byteSpan[offset];

                        int sample = AdpcmMsExpandNibble(ref channel0, nibbles >> 4);
                        samples[sampleOffset] = (byte)sample;
                        samples[sampleOffset + 1] = (byte)(sample >> 8);

                        sample = AdpcmMsExpandNibble(ref channel1, nibbles & 0x0f);
                        samples[sampleOffset + 2] = (byte)sample;
                        samples[sampleOffset + 3] = (byte)(sample >> 8);

                        sampleOffset += 4;
                        ++offset;
                    }
                }
                else
                {
                    for (int i = 0; i < blockSize; ++i)
                    {
                        int nibbles = byteSpan[offset];

                        int sample = AdpcmMsExpandNibble(ref channel0, nibbles >> 4);
                        samples[sampleOffset] = (byte)sample;
                        samples[sampleOffset + 1] = (byte)(sample >> 8);

                        sample = AdpcmMsExpandNibble(ref channel0, nibbles & 0x0f);
                        samples[sampleOffset + 2] = (byte)sample;
                        samples[sampleOffset + 3] = (byte)(sample >> 8);

                        sampleOffset += 4;
                        ++offset;
                    }
                }
            }
            return samples;
        }

        #endregion
    }
}
