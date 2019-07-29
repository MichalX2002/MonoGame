using System;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Utilities.Memory;
using NVorbis;

namespace MonoGame.Framework.Audio
{
    internal static partial class AudioLoader
    {
        private static MemoryStream LoadVorbis(
            Stream stream, out ALFormat format, out int frequency, out int channels, 
            out int blockAlignment, out int bitsPerSample, out int samplesPerBlock, out int sampleCount)
        {
            byte[] bufferBlock = null;
            MemoryStream result = null;
            var reader = new VorbisReader(stream, leaveOpen: false);
            try
            {
                bool floatOutput = ALController.Instance.SupportsFloat32;
                int sampleSize = floatOutput ? 4 : 2; // sizeof(float) : sizeof(short);

                channels = reader.Channels;
                if (channels == 1)
                    format = floatOutput ? ALFormat.MonoFloat32 : ALFormat.Mono16;
                else if (channels == 2)
                    format = floatOutput ? ALFormat.StereoFloat32 : ALFormat.Stereo16;
                else
                    throw new NotSupportedException("Only mono and stereo is supported.");

                sampleCount = (int)reader.TotalSamples;
                frequency = reader.SampleRate;
                blockAlignment = 0;
                samplesPerBlock = 0;
                bitsPerSample = sampleSize * 8;

                long outputBytes = sampleCount * channels * sampleSize;
                if (outputBytes > int.MaxValue)
                    throw new InvalidDataException("Size of decoded audio data exceeds " + int.MaxValue + " bytes.");

                result = RecyclableMemoryManager.Instance.GetMemoryStream(null, (int)outputBytes);
                bufferBlock = RecyclableMemoryManager.Instance.GetBlock();

                Span<byte> bufferBlockBytes = bufferBlock.AsSpan();
                Span<short> bufferBlockShorts = MemoryMarshal.Cast<byte, short>(bufferBlockBytes);
                
                int sampleBufferSize = bufferBlock.Length / 4;
                Span<float> sampleBuffer = sampleBufferSize <= 1024 * 20 ? 
                    stackalloc float[sampleBufferSize] : new float[sampleBufferSize];
                Span<byte> sampleBufferBytes = MemoryMarshal.AsBytes(sampleBuffer);

                int totalSamples = 0;
                int samplesRead;
                while ((samplesRead = reader.ReadSamples(sampleBuffer)) > 0)
                {
                    if (floatOutput)
                    {
                        // we can copy directly to output
                        int bytes = samplesRead * sizeof(float);
                        var src = sampleBufferBytes.Slice(0, bytes);
                        src.CopyTo(bufferBlockBytes);
                        result.Write(bufferBlock, 0, bytes);
                    }
                    else
                    {
                        // we need to convert float to short
                        var src = sampleBuffer.Slice(0, samplesRead);
                        var dst = bufferBlockShorts.Slice(0, samplesRead);
                        ConvertSamplesToInt16(src, dst);

                        int bytes = samplesRead * sizeof(short);
                        result.Write(bufferBlock, 0, bytes);
                    }
                    totalSamples += samplesRead;
                }

                long readerSamples = reader.TotalSamples;
                if (readerSamples != long.MaxValue && totalSamples < readerSamples)
                    throw new InvalidDataException(
                        "Reached end of stream before reading expected amount of samples.");

                result.Position = 0;
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
            finally
            {
                if (bufferBlock != null)
                    RecyclableMemoryManager.Instance.ReturnBlock(bufferBlock);
                reader?.Dispose();
            }
        }
    }
}