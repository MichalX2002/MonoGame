using System;
using System.Runtime.InteropServices;
using Android.Media;
using Java.IO;
using Java.Nio;
using Microsoft.Xna.Framework.Content;

namespace Microsoft.Xna.Framework.Audio
{
    public class AudioDecoder
    {
        public struct Result
        {
            public byte[] Samples { get; }
            public int SampleRate { get; }
            public int Channels { get; }
            public string Mime { get; }

            public Result(byte[] samples, int sampleRate, int channels, string mime)
            {
                Samples = samples;
                SampleRate = sampleRate;
                Channels = channels;
                Mime = mime;
            }
        }

        public static Result DecodeAudio(FileDescriptor descriptor, long offset, long length)
        {
            using (var extractor = new MediaExtractor())
            {
                extractor.SetDataSource(descriptor, offset, length);

                MediaFormat format = null;
                string mime = null;
                for (int i = 0; i < extractor.TrackCount; i++)
                {
                    format = extractor.GetTrackFormat(i);
                    mime = format.GetString(MediaFormat.KeyMime);

                    if (!mime.StartsWith("audio/"))
                        continue;
                    extractor.SelectTrack(i);
                }

                if (format == null || !mime.StartsWith("audio/"))
                    throw new ContentLoadException("Could not find any audio track.");

                int sampleRate = format.GetInteger(MediaFormat.KeySampleRate);
                long duration = format.GetLong(MediaFormat.KeyDuration);
                int channels = format.GetInteger(MediaFormat.KeyChannelCount);

                int samples = (int)(sampleRate * duration / 1000000d);
                var output = new byte[samples * 2];
                int timeoutsLeft = 1000;

                using (var decoder = MediaCodec.CreateDecoderByType(mime))
                {
                    decoder.Configure(format, null, null, MediaCodecConfigFlags.None);
                    decoder.Start();

                    var bufferInfo = new MediaCodec.BufferInfo();
                    int totalOffset = 0;
                    bool endOfStream = false;
                    while (true)
                    {
                        int decoderStatus = decoder.DequeueOutputBuffer(bufferInfo, 10000);
                        if (decoderStatus == (int)MediaCodecInfoState.TryAgainLater)
                        {
                            if (timeoutsLeft-- <= 0)
                                break;
                        }
                        else if (decoderStatus >= 0)
                        {
                            ByteBuffer[] outputBuffers = decoder.GetOutputBuffers();
                            IntPtr bufferPtr = outputBuffers[decoderStatus].GetDirectBufferAddress();
                            IntPtr ptr = bufferPtr + bufferInfo.Offset;

                            int size = bufferInfo.Size;
                            Marshal.Copy(ptr, output, totalOffset, size);

                            decoder.ReleaseOutputBuffer(decoderStatus, render: false);
                            totalOffset += size;
                        }

                        if (endOfStream)
                        {
                            if (totalOffset != output.Length)
                                throw new ContentLoadException(
                                    "Reached end of stream before reading expected amount of samples.");
                            break;
                        }

                        int inputBufIndex = decoder.DequeueInputBuffer(10000);
                        if (inputBufIndex == -1)
                        {
                            if (timeoutsLeft-- <= 0)
                                break;
                            continue;
                        }
                        else if (inputBufIndex >= 0)
                        {
                            ByteBuffer[] inputBuffers = decoder.GetInputBuffers();
                            int size = extractor.ReadSampleData(inputBuffers[inputBufIndex], 0);
                            if (size > 0)
                                decoder.QueueInputBuffer(
                                    inputBufIndex, 0, size, extractor.SampleTime, MediaCodecBufferFlags.None);

                            if (!extractor.Advance())
                                endOfStream = true;
                        }
                    }
                }

                if (timeoutsLeft <= 0)
                    throw new ContentLoadException("Could not load sound effect in designated time frame.");
                return new Result(output, sampleRate, channels, mime);
            }
        }
    }
}