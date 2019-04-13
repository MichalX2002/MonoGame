using Microsoft.Xna.Framework.Audio;
using MonoGame.OpenAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Xna.Framework.Media
{
    internal class OggStreamer : IDisposable
    {
        public readonly EffectsExtension Efx = ALController.Efx;

        const float DefaultUpdateRate = 10;
        const int DefaultBufferSize = 8000;
        const int MaxBuffers = 3;

        internal static readonly object _singletonMutex = new object();
        readonly object _iterationMutex = new object();
        readonly object _readMutex = new object();

        readonly float[] _readBuffer;
        readonly short[] _castBuffer;
        readonly HashSet<OggStream> _streams;
        readonly List<TimeSpan> _threadTiming;

        readonly Thread _thread;
        Stopwatch _threadWatch;
        bool _pendingFinish;
        volatile bool _cancelled;

        public float UpdateRate { get; }
        public int BufferSize { get; }
        public ReadOnlyCollection<TimeSpan> ThreadTiming { get; }

        private static OggStreamer _instance;
        public static OggStreamer Instance
        {
            get
            {
                lock (_singletonMutex)
                {
                    if (_instance == null)
                        throw new InvalidOperationException("No instance running.");
                    return _instance;
                }
            }
            private set
            {
                lock (_singletonMutex)
                    _instance = value;
            }
        }

        public OggStreamer(int bufferSize = DefaultBufferSize, float updateRate = DefaultUpdateRate)
        {
            UpdateRate = updateRate;
            BufferSize = bufferSize;
            _pendingFinish = false;

            lock (_singletonMutex)
            {
                if (_instance != null)
                    throw new InvalidOperationException("Already running.");
                Instance = this;

                _threadWatch = new Stopwatch();
                _threadTiming = new List<TimeSpan>((int)(UpdateRate < 1 ? 1 : UpdateRate));
                ThreadTiming = _threadTiming.AsReadOnly();

                _thread = new Thread(EnsureBuffersFilled)
                {
                    Name = "Song Streaming Thread",
                    Priority = ThreadPriority.BelowNormal
                };
                _thread.Start();
            }

            _readBuffer = new float[BufferSize];
            _castBuffer = new short[BufferSize];
            _streams = new HashSet<OggStream>();
        }

        public void Dispose()
        {
            lock (_singletonMutex)
            {
                Debug.Assert(Instance == this, "Two instances running, somehow...?");

                _cancelled = true;
                lock (_iterationMutex)
                    _streams.Clear();

                _thread.Join(1000);
                Instance = null;
            }
        }

        internal bool AddStream(OggStream stream)
        {
            lock (_iterationMutex)
                return _streams.Add(stream);
        }

        internal bool RemoveStream(OggStream stream)
        {
            lock (_iterationMutex)
                return _streams.Remove(stream);
        }

        public bool TryReadBuffer(OggStream stream, out ALBuffer buffer)
        {
            lock (_readMutex)
            {
                var reader = stream.Reader;
                int readSamples = reader.ReadSamples(_readBuffer);

                if (readSamples > 0)
                {
                    buffer = ALBufferPool.Rent();

                    var channels = (AudioChannels)reader.Channels;
                    bool useFloat = ALController.Instance.SupportsFloat32;
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
                    return true;
                }
            }

            buffer = null;
            return false;
        }

        static void CastBuffer(float[] src, short[] dst, int count)
        {
            for (int i = 0; i < count; i++)
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

        void EnsureBuffersFilled()
        {
            var localStreams = new List<OggStream>();

            while (!_cancelled)
            {
                Thread.Sleep((int)(1000 / ((UpdateRate <= 0) ? 1 : UpdateRate)));
                if (_cancelled)
                    break;

                _threadWatch.Restart();

                localStreams.Clear();
                lock (_iterationMutex)
                {
                    foreach(var stream in _streams)
                        localStreams.Add(stream);
                }

                foreach (OggStream stream in localStreams)
                {
                    lock (stream._prepareMutex)
                    {
                        lock (_iterationMutex)
                            if (!_streams.Contains(stream))
                                continue;

                        if (!FillStream(stream))
                            continue;
                    }

                    lock (stream._stopMutex)
                    {
                        if (stream.IsPreparing)
                            continue;

                        lock (_iterationMutex)
                            if (!_streams.Contains(stream))
                                continue;

                        var state = AL.GetSourceState(stream._alSourceID);
                        ALHelper.CheckError("Failed to get source state.");

                        if (state == ALSourceState.Stopped)
                        {
                            AL.SourcePlay(stream._alSourceID);
                            ALHelper.CheckError("Failed to play.");
                        }
                    }
                }

                _threadWatch.Stop();
                _threadTiming.Add(_threadWatch.Elapsed);
                if (_threadTiming.Count >= _threadTiming.Capacity)
                    _threadTiming.RemoveAt(0);
            }
        }

        private bool FillStream(OggStream stream)
        {
            AL.GetSource(stream._alSourceID, ALGetSourcei.BuffersQueued, out int queued);
            ALHelper.CheckError("Failed to fetch queued buffers.");

            AL.GetSource(stream._alSourceID, ALGetSourcei.BuffersProcessed, out int processed);
            ALHelper.CheckError("Failed to fetch processed buffers.");

            int requested = Math.Max(MaxBuffers - stream.BufferCount, 0);
            if (processed == 0 && requested == 0)
                return false;
            
            if (processed > 0)
            {
                AL.SourceUnqueueBuffers(stream._alSourceID, processed);
                ALHelper.CheckError("Failed to unqueue buffers.");

                for (int i = 0; i < processed; i++)
                    stream.DequeueAndReturnBuffer();
            }

            for (int i = 0; i < requested; i++)
            {
                if (TryReadBuffer(stream, out ALBuffer buffer))
                {
                    stream.EnqueueBuffer(buffer);
                }
                else
                {
                    if (stream.IsLooped)
                    {
                        // closing and opening the stream is a bit expensive
                        //stream.Close();
                        //stream.Open();

                        // we dont support non-seekable streams anyway
                        stream.SeekToPosition(0);
                    }
                    else
                    {
                        _pendingFinish = true;
                        break;
                    }
                }
            }

            if (_pendingFinish && queued == 0)
            {
                _pendingFinish = false;
                lock (_iterationMutex)
                    _streams.Remove(stream);

                if (stream.OnFinished != null)
                    stream.OnFinished.Invoke();
            }
            else if (!stream.IsLooped)
            {
                return false;
            }
            return true;
        }
    }
}
