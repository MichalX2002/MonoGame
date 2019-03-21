using Microsoft.Xna.Framework.Audio;
using MonoGame.OpenAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Xna.Framework.Media
{
    internal class OggStreamer : IDisposable
    {
        public readonly EffectsExtension Efx = ALSoundController.Efx;

        const float DefaultUpdateRate = 16;
        const int DefaultBufferSize = 44100 * 2;

        internal static readonly object singletonMutex = new object();
        readonly object iterationMutex = new object();
        readonly object readMutex = new object();

        readonly float[] _readBuffer;
        readonly short[] _castBuffer;

        readonly HashSet<OggStream> _streams = new HashSet<OggStream>();

        readonly Thread _thread;
        Stopwatch threadWatch;
        int nextTimingIndex;
        bool pendingFinish;
        volatile bool cancelled;

        public float UpdateRate { get; }
        public int BufferSize { get; }
        public float[] ThreadTiming { get; }

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

                _thread = new Thread(EnsureBuffersFilled)
                {
                    Name = "Song Streaming Thread",
                    Priority = ThreadPriority.BelowNormal
                };
                _thread.Start();
            }

            _readBuffer = new float[BufferSize];
            _castBuffer = new short[BufferSize];
        }

        public void Dispose()
        {
            lock (singletonMutex)
            {
                Debug.Assert(Instance == this, "Two instances running, somehow...?");

                cancelled = true;
                lock (iterationMutex)
                    _streams.Clear();

                _thread.Join(1000);
                Instance = null;
            }
        }

        internal bool AddStream(OggStream stream)
        {
            lock (iterationMutex)
                return _streams.Add(stream);
        }

        internal bool RemoveStream(OggStream stream)
        {
            lock (iterationMutex)
                return _streams.Remove(stream);
        }

        public bool FillBuffer(OggStream stream, ALSoundBuffer buffer)
        {
            lock (readMutex)
            {
                var reader = stream.Reader;
                int readSamples = reader.ReadSamples(_readBuffer, 0, BufferSize);

                if (readSamples > 0)
                {
                    var channels = (AudioChannels)reader.Channels;
                    bool useFloat = ALSoundController.Instance.SupportsFloat32;
                    ALFormat format = ALHelper.GetALFormat(channels, useFloat);

                    if (useFloat)
                    {
                        buffer.BufferData(_readBuffer, readSamples, format, reader.SampleRate);
                    }
                    else
                    {
                        CastBuffer(_readBuffer, _castBuffer, readSamples);
                        buffer.BufferData(_castBuffer, readSamples, format, reader.SampleRate);
                    }
                }
                return readSamples == 0;
            }
        }

        static void CastBuffer(float[] src, short[] dst, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int tmp = (int)(32767f * src[i]);
                if (tmp > short.MaxValue)
                    tmp = short.MaxValue;
                else if (tmp < short.MinValue)
                    tmp = short.MinValue;
                dst[i] = (short)tmp;
            }
        }

        void EnsureBuffersFilled()
        {
            var localStreams = new List<OggStream>();

            while (!cancelled)
            {
                Thread.Sleep((int)(1000 / ((UpdateRate <= 0) ? 1 : UpdateRate)));
                if (cancelled)
                    break;

                threadWatch.Restart();

                localStreams.Clear();
                lock (iterationMutex)
                {
                    foreach(var stream in _streams)
                        localStreams.Add(stream);
                }

                foreach (OggStream stream in localStreams)
                {
                    lock (stream._prepareMutex)
                    {
                        lock (iterationMutex)
                            if (!_streams.Contains(stream))
                                continue;

                        if (!FillStream(stream))
                            continue;
                    }

                    lock (stream._stopMutex)
                    {
                        if (stream.IsPreparing)
                            continue;

                        lock (iterationMutex)
                            if (!_streams.Contains(stream))
                                continue;

                        var state = AL.GetSourceState(stream._alSourceId);
                        ALHelper.CheckError("Failed to get source state.");

                        if (state == ALSourceState.Stopped)
                        {
                            AL.SourcePlay(stream._alSourceId);
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

        private bool FillStream(OggStream stream)
        {
            AL.GetSource(stream._alSourceId, ALGetSourcei.BuffersQueued, out int queued);
            ALHelper.CheckError("Failed to fetch queued buffers.");

            AL.GetSource(stream._alSourceId, ALGetSourcei.BuffersProcessed, out int processed);
            ALHelper.CheckError("Failed to fetch processed buffers.");

            if (processed == 0 && queued == stream.BufferCount)
                return false;

            int[] tmpBuffers;
            int tmpBufferOffset;
            int tmpBufferCount;

            if (processed > 0)
            {
                tmpBuffers = AL.SourceGetAndUnqueueBuffers(stream._alSourceId, processed);
                tmpBufferOffset = 0;
                tmpBufferCount = tmpBuffers.Length;
                ALHelper.CheckError("Failed to unqueue buffers.");
            }
            else
            {
                tmpBuffers = stream._alBufferIds;
                tmpBufferOffset = queued;
                tmpBufferCount = stream._alBufferIds.Length;
            }

            bool finished = false;
            int buffersFilled = 0;
            for (int i = tmpBufferOffset; i < tmpBufferCount; i++)
            {
                finished |= FillBuffer(stream, tmpBuffers[i]);
                buffersFilled++;

                if (finished)
                {
                    if (stream.IsLooped)
                    {
                        // we dont support non-seekable streams anyway
                        stream.SeekToPosition(0);

                        // closing and opening the stream is a bit expensive
                        //stream.Close();
                        //stream.Open();
                    }
                    else
                    {
                        pendingFinish = true;
                        break;
                    }
                }
            }

            if (pendingFinish && queued == 0)
            {
                pendingFinish = false;
                lock (iterationMutex)
                    _streams.Remove(stream);

                if (stream.OnFinished != null)
                    stream.OnFinished.Invoke();
            }
            else if (!finished && buffersFilled > 0) // queue only successfully filled buffers
            {
                for (int i = tmpBufferOffset; i < tmpBufferCount; i++)
                {
                    if (buffersFilled <= 0)
                        break;
                    buffersFilled--;

                    AL.SourceQueueBuffer(stream._alSourceId, tmpBuffers[i]);
                    ALHelper.CheckError("Failed to queue buffers.");
                }
            }
            else if (!stream.IsLooped)
                return false;

            return true;
        }
    }
}
