using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Framework.Memory;
using MonoGame.OpenAL;
using NVorbis;

namespace MonoGame.Framework.Audio
{
    internal static partial class AudioLoader
    {
        [SkipLocalsInit]
        private static RecyclableBuffer LoadVorbis(
            Stream stream, out ALFormat format, out int frequency, out int channels,
            out int blockAlignment, out int bitsPerSample, out int samplesPerBlock, out int sampleCount)
        {
            RecyclableBuffer? result = null;
            var reader = new VorbisReader(stream, leaveOpen: false);
            try
            {
                bool useFloat = ALController.Get().SupportsFloat32;
                int sampleSize = useFloat ? sizeof(float) : sizeof(short);

                channels = reader.Channels;
                if (channels == 1)
                    format = useFloat ? ALFormat.MonoFloat32 : ALFormat.Mono16;
                else if (channels == 2)
                    format = useFloat ? ALFormat.StereoFloat32 : ALFormat.Stereo16;
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

                result = RecyclableMemoryManager.Default.GetBuffer((int)outputBytes, "Vorbis audio data");

                var resultSpan = result.AsSpan();
                int totalSamplesRead = 0;
                int samplesRead;

                // Both paths allocate around 4096 stack bytes for buffers.
                if (useFloat)
                {
                    Span<float> sampleBuffer = stackalloc float[1024];
                    Span<byte> sampleBufferBytes = MemoryMarshal.AsBytes(sampleBuffer);

                    // we can copy directly to output
                    while ((samplesRead = reader.ReadSamples(sampleBuffer)) > 0)
                    {
                        Span<byte> bytes = sampleBufferBytes.Slice(0, samplesRead * sizeof(float));
                        bytes.CopyTo(resultSpan);
                        resultSpan = resultSpan[bytes.Length..];
                        totalSamplesRead += samplesRead;
                    }
                }
                else
                {
                    // The buffer lengths are multiples of 16 to allow for better vectorization.
                    Span<float> sampleBuffer = stackalloc float[672];
                    Span<short> pcmBuffer = stackalloc short[672];
                    Span<byte> sampleBufferBytes = MemoryMarshal.AsBytes(sampleBuffer);
                    Span<byte> pcmBufferBytes = MemoryMarshal.AsBytes(pcmBuffer);

                    // we need to convert float to short
                    while ((samplesRead = reader.ReadSamples(sampleBuffer)) > 0)
                    {
                        var src = sampleBuffer.Slice(0, samplesRead);
                        var dst = pcmBuffer.Slice(0, samplesRead);
                        ConvertSingleToInt16(src, dst);

                        Span<byte> bytes = pcmBufferBytes.Slice(0, samplesRead * sizeof(short));
                        bytes.CopyTo(resultSpan);
                        resultSpan = resultSpan[bytes.Length..];
                        totalSamplesRead += samplesRead;
                    }
                }

                if (!resultSpan.IsEmpty)
                    throw new InvalidDataException(
                        "Reached end of stream before reading expected amount of samples.");

                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
            finally
            {
                reader?.Dispose();
            }
        }
    }
}