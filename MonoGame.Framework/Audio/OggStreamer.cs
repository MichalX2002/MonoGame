using MonoGame.OpenAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.Xna.Framework.Audio
{
    internal class OggStreamer : IDisposable
    {
        public readonly XRamExtension XRam = new XRamExtension();
        public readonly EffectsExtension Efx = OpenALSoundController.Efx;

        const float DefaultUpdateRate = 10;
        const int DefaultBufferSize = 44100;

        static readonly object singletonMutex = new object();

        readonly object iterationMutex = new object();
        readonly object readMutex = new object();

        readonly float[] readSampleBuffer;
        readonly short[] castBuffer;

        readonly HashSet<OggStream> streams = new HashSet<OggStream>();
        readonly List<OggStream> threadLocalStreams = new List<OggStream>();

        readonly Thread underlyingThread;
        Stopwatch threadWatch;
        int nextTimingIndex;
        volatile bool cancelled;
        bool pendingFinish;

        public float UpdateRate { get; private set; }
        public int BufferSize { get; private set; }

        private static OggStreamer _instance;
        public static OggStreamer Instance
        {
            get
            {
                lock (singletonMutex)
                {
                    if (_instance == null)
                        throw new InvalidOperationException("No instance running.");
                    return _instance;
                }
            }
            private set
            {
                lock (singletonMutex)
                    _instance = value;
            }
        }

        public float[] ThreadTiming { get; }

        public OggStreamer(int bufferSize = DefaultBufferSize, float updateRate = DefaultUpdateRate)
        {
            UpdateRate = updateRate;
            BufferSize = bufferSize;
            pendingFinish = false;

            lock (singletonMutex)
            {
                if (_instance != null)
                    throw new InvalidOperationException("Already running.");
                Instance = this;

                threadWatch = new Stopwatch();
                ThreadTiming = new float[(int)(UpdateRate < 1 ? 1 : UpdateRate)];

                underlyingThread = new Thread(EnsureBuffersFilled) { Priority = ThreadPriority.BelowNormal };
                underlyingThread.Start();
            }

            readSampleBuffer = new float[bufferSize];
            castBuffer = new short[bufferSize];
        }

        public void Dispose()
        {
            lock (singletonMutex)
            {
                Debug.Assert(Instance == this, "Two instances running, somehow...?");

                cancelled = true;
                lock (iterationMutex)
                    streams.Clear();

                Instance = null;
            }
        }

        internal bool AddStream(OggStream stream)
        {
            lock (iterationMutex)
                return streams.Add(stream);
        }

        internal bool RemoveStream(OggStream stream)
        {
            lock (iterationMutex)
                return streams.Remove(stream);
        }

        public bool FillBuffer(OggStream stream, int bufferId)
        {
            int readSamples;
            lock (readMutex)
            {
                readSamples = stream.Reader.ReadSamples(readSampleBuffer, 0, BufferSize);
                CastBuffer(readSampleBuffer, castBuffer, readSamples);
            }

            if (readSamples > 0)
            {
                var format = stream.Reader.Channels == 1 ? ALFormat.Mono16 : ALFormat.Stereo16;
                int size = readSamples * sizeof(short);
                AL.BufferData(bufferId, format, castBuffer, size, stream.Reader.SampleRate);
                ALHelper.CheckError("Failed to fill buffer, readSamples = {0}, SampleRate = {1}, buffer.Length = {2}.", readSamples, stream.Reader.SampleRate, castBuffer.Length);
            }

            return readSamples != BufferSize;
        }

        static void CastBuffer(float[] inBuffer, short[] outBuffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                int temp = (int)(32767f * inBuffer[i]);
                if (temp > short.MaxValue)
                    temp = short.MaxValue;
                else if (temp < short.MinValue)
                    temp = short.MinValue;
                outBuffer[i] = (short)temp;
            }
        }

        void EnsureBuffersFilled()
        {
            while (!cancelled)
            {
                Thread.Sleep((int)(1000 / ((UpdateRate <= 0) ? 1 : UpdateRate)));
                if (cancelled)
                    break;

                threadWatch.Restart();

                threadLocalStreams.Clear();
                lock (iterationMutex)
                    threadLocalStreams.AddRange(streams);

                foreach (OggStream stream in threadLocalStreams)
                {
                    lock (stream.prepareMutex)
                    {
                        lock (iterationMutex)
                            if (!streams.Contains(stream))
                                continue;

                        AL.GetSource(stream.alSourceId, ALGetSourcei.BuffersQueued, out int queued);
                        ALHelper.CheckError("Failed to fetch queued buffers.");

                        AL.GetSource(stream.alSourceId, ALGetSourcei.BuffersProcessed, out int processed);
                        ALHelper.CheckError("Failed to fetch processed buffers.");

                        if (processed == 0 && queued == stream.BufferCount)
                            continue;

                        IEnumerable<int> tempBuffers;
                        if (processed > 0)
                        {
                            tempBuffers = AL.SourceUnqueueBuffers(stream.alSourceId, processed);
                            ALHelper.CheckError("Failed to unqueue buffers.");
                        }
                        else
                            tempBuffers = stream.alBufferIds.Skip(queued);

                        bool finished = false;
                        int buffersFilled = 0;
                        foreach(int buffer in tempBuffers)
                        {
                            if (pendingFinish)
                                break;

                            finished |= FillBuffer(stream, buffer);
                            buffersFilled++;

                            if (finished)
                            {
                                if (stream.IsLooped)
                                {
                                    if (stream.CanSeek)
                                    {
                                        // try to seek first
                                        stream.SeekToPosition(0);
                                    }
                                    else
                                    { 
                                        // as closing and opening the stream is a bit expensive
                                        stream.Close();
                                        stream.Open();
                                    }
                                }
                                else
                                    pendingFinish = true;
                            }
                        }

                        if (pendingFinish && queued == 0)
                        {
                            pendingFinish = false;
                            lock (iterationMutex)
                                streams.Remove(stream);

                            if (stream.FinishedAction != null)
                                stream.FinishedAction.Invoke();
                        }
                        else if (!finished && buffersFilled > 0) // queue only successfully filled buffers
                        {
                            foreach (int buffer in tempBuffers)
                            {
                                if (buffersFilled <= 0)
                                    break;
                                buffersFilled--;

                                AL.SourceQueueBuffer(stream.alSourceId, buffer);
                                ALHelper.CheckError("Failed to queue buffers.");
                            }
                        }
                        else if (!stream.IsLooped)
                            continue;
                    }

                    lock (stream.stopMutex)
                    {
                        if (stream.Preparing)
                            continue;

                        lock (iterationMutex)
                            if (!streams.Contains(stream))
                                continue;

                        var state = AL.GetSourceState(stream.alSourceId);
                        ALHelper.CheckError("Failed to get source state.");

                        if (state == ALSourceState.Stopped)
                        {
                            AL.SourcePlay(stream.alSourceId);
                            ALHelper.CheckError("Failed to play.");
                        }
                    }
                }

                threadWatch.Stop();
                ThreadTiming[nextTimingIndex] = (float)threadWatch.Elapsed.TotalSeconds;

                nextTimingIndex++;
                if (nextTimingIndex >= ThreadTiming.Length)
                    nextTimingIndex = 0;
            }
        }
    }
}
