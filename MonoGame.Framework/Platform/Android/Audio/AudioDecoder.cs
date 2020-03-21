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

                var decoder = MediaCodecPool.RentDecoder(mime);
                try
                {
                    decoder.Configure(format, null, null, MediaCodecConfigFlags.None);
                    decoder.Start();

                    ByteBuffer[] inputBuffers = decoder.GetInputBuffers();
                    ByteBuffer[] outputBuffers = decoder.GetOutputBuffers();

                    var bufferInfo = new MediaCodec.BufferInfo();
                    int totalOffset = 0;
                    bool endOfStream = false;
                    while (true)
                    {
                        // we dont need to have a endOfStream local,
                        // but it saves us a few calls to the decoder
                        if (!endOfStream)
                        {
                            int inputBufIndex = decoder.DequeueInputBuffer(5000);
                            if (inputBufIndex >= 0)
                            {
                                int size = extractor.ReadSampleData(inputBuffers[inputBufIndex], 0);
                                if (size > 0)
                                {
                                    decoder.QueueInputBuffer(
                                        inputBufIndex, 0, size, extractor.SampleTime, MediaCodecBufferFlags.None);
                                }

                                if (!extractor.Advance())
                                {
                                    endOfStream = true;
                                    decoder.QueueInputBuffer(
                                        inputBufIndex, 0, 0, 0, MediaCodecBufferFlags.EndOfStream);
                                }
                            }
                        }

                        int decoderStatus = decoder.DequeueOutputBuffer(bufferInfo, 5000);
                        if (decoderStatus >= 0)
                        {
                            IntPtr bufferPtr = outputBuffers[decoderStatus].GetDirectBufferAddress();
                            IntPtr offsetPtr = bufferPtr + bufferInfo.Offset;
                            int size = bufferInfo.Size;
                            Marshal.Copy(offsetPtr, output, totalOffset, size);

                            decoder.ReleaseOutputBuffer(decoderStatus, render: false);
                            totalOffset += size;

                            if (bufferInfo.Flags == MediaCodecBufferFlags.EndOfStream)
                            {
                                if (totalOffset != output.Length)
                                    throw new ContentLoadException(
                                        "Reached end of stream before reading expected amount of samples.");
                                break;
                            }
                        }
                        else if (decoderStatus == (int)MediaCodecInfoState.OutputBuffersChanged)
                        {
                            outputBuffers = decoder.GetOutputBuffers();
                        }
                        else if (decoderStatus == (int)MediaCodecInfoState.TryAgainLater)
                        {
                            if (timeoutsLeft-- <= 0)
                                break;
                        }
                    }
                }
                finally
                {
                    decoder.Stop();
                    MediaCodecPool.ReturnDecoder(mime, decoder);
                }

                if (timeoutsLeft <= 0)
                    throw new ContentLoadException("Could not load sound effect in designated time frame.");
                return new Result(output, sampleRate, channels, mime);
            }
        }

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
    }
}