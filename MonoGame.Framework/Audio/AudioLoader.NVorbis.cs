using System;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Utilities.Memory;
using NVorbis;

namespace Microsoft.Xna.Framework.Audio
{
    internal static partial class AudioLoader
    {
        private static byte[] LoadVorbis(
            Stream stream, out ALFormat format, out int frequency, out int channels, 
            out int blockAlignment, out int bitsPerSample, out int samplesPerBlock, out int sampleCount)
        {
            byte[] block = null;
            var reader = new VorbisReader(stream, leaveOpen: false);
            try
            {
                sampleCount = (int)reader.TotalSamples;
                channels = reader.Channels;
                frequency = reader.SampleRate;
                blockAlignment = 0;
                samplesPerBlock = 0;

                bool floatOutput = ALController.Instance.SupportsFloat32;
                int sampleSize = (floatOutput ? sizeof(float) : sizeof(short));
                bitsPerSample = sampleSize * 8;

                long outputBytes = sampleCount * channels * sampleSize;
                if (outputBytes > int.MaxValue)
                    throw new InvalidDataException("Size of decoded audio data exceeds " + int.MaxValue + " bytes.");

                if (channels == 1)
                    format = floatOutput ? ALFormat.MonoFloat32 : ALFormat.Mono16;
                else if (channels == 2)
                    format = floatOutput ? ALFormat.StereoFloat32 : ALFormat.Stereo16;
                else
                    throw new NotSupportedException("Only mono and stereo is supported.");

                block = RecyclableMemoryManager.Instance.GetBlock();
                Span<byte> blockBytes = block.AsSpan();
                Span<short> blockShorts = MemoryMarshal.Cast<byte, short>(blockBytes);

                Span<float> sampleBuffer = stackalloc float[block.Length / 4];
                Span<byte> sampleBufferBytes = MemoryMarshal.AsBytes(sampleBuffer);

                using (var memoryBuffer = RecyclableMemoryManager.Instance.GetMemoryStream(null, (int)outputBytes))
                {
                    int totalSamples = 0;
                    int samplesRead;
                    while ((samplesRead = reader.ReadSamples(sampleBuffer)) > 0)
                    {
                        if (floatOutput)
                        {
                            // we can copy directly to output
                            int len = samplesRead * sizeof(float);
                            var src = sampleBufferBytes.Slice(0, len);
                            src.CopyTo(blockBytes);
                            memoryBuffer.Write(block, 0, len);
                        }
                        else
                        {
                            // we need to convert float to short
                            for (int i = 0; i < samplesRead; i++)
                            {
                                int tmp = (int)(32767f * sampleBuffer[i]);
                                if (tmp > short.MaxValue)
                                    blockShorts[i] = short.MaxValue;
                                else if (tmp < short.MinValue)
                                    blockShorts[i] = short.MinValue;
                                else
                                    blockShorts[i] = (short)tmp;
                            }

                            int len = samplesRead * sizeof(short);
                            memoryBuffer.Write(block, 0, len);
                        }
                        totalSamples += samplesRead;
                    }

                    if (totalSamples != reader.TotalSamples)
                        throw new InvalidDataException(
                            "Reached end of stream before reading expected amount of samples.");

                    return memoryBuffer.ToArray();
                }
            }
            finally
            {
                if(block != null)
                    RecyclableMemoryManager.Instance.ReturnBlock(block);
                reader?.Dispose();
            }
        }
    }
}