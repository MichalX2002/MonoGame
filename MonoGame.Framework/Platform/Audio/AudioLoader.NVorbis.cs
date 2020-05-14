using System;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Framework.Memory;
using MonoGame.OpenAL;
using NVorbis;

namespace MonoGame.Framework.Audio
{
    internal static partial class AudioLoader
    {
        private static RecyclableBuffer LoadVorbis(
            Stream stream, out ALFormat format, out int frequency, out int channels,
            out int blockAlignment, out int bitsPerSample, out int samplesPerBlock, out int sampleCount)
        {
            byte[] pcmBufferBlock = null;
            byte[] floatBufferBlock = null;
            RecyclableBuffer result = null;
            var reader = new VorbisReader(stream, leaveOpen: false);
            try
            {
                bool useFloat = ALController.Instance.SupportsFloat32;
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
                pcmBufferBlock = RecyclableMemoryManager.Default.GetBlock();
                floatBufferBlock = RecyclableMemoryManager.Default.GetBlock();

                // First cast then use AsBytes to prevent misalignment
                Span<float> sampleBuffer = MemoryMarshal.Cast<byte, float>(floatBufferBlock);
                Span<byte> sampleBufferBytes = MemoryMarshal.AsBytes(sampleBuffer);
                Span<short> pcmBuffer = MemoryMarshal.Cast<byte, short>(pcmBufferBlock);
                Span<byte> pcmBufferBytes = MemoryMarshal.AsBytes(pcmBuffer);

                var resultSpan = result.AsSpan();
                int totalSamples = 0;
                int samplesRead;
                while ((samplesRead = reader.ReadSamples(sampleBuffer)) > 0)
                {
                    Span<byte> bytes;
                    if (useFloat)
                    {
                        // we can copy directly to output
                        bytes = sampleBufferBytes.Slice(0, samplesRead * sizeof(float));
                    }
                    else
                    {
                        // we need to convert float to short
                        var src = sampleBuffer.Slice(0, samplesRead);
                        var dst = pcmBuffer.Slice(0, samplesRead);
                        ConvertSingleToInt16(src, dst);

                        bytes = pcmBufferBytes.Slice(0, samplesRead * sizeof(short));
                    }

                    bytes.CopyTo(resultSpan);
                    resultSpan = resultSpan.Slice(bytes.Length);
                    totalSamples += samplesRead;
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
                if (pcmBufferBlock != null)
                    RecyclableMemoryManager.Default.ReturnBlock(pcmBufferBlock);
                if (floatBufferBlock != null)
                    RecyclableMemoryManager.Default.ReturnBlock(floatBufferBlock);
                reader?.Dispose();
            }
        }
    }
}