using MonoGame.Framework.Audio;
using MonoGame.OpenAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MonoGame.Framework.Media
{
    internal class OggStreamer : IDisposable
    {
        public const float DefaultUpdateRate = 16;
        public const int DefaultBufferSize = 8000;
        public const int MaxQueuedBuffers = 4;

        internal static object SingletonMutex { get; } = new object();

        internal object IterationMutex { get; } = new object();
        private object FillMutex { get; } = new object();

        private static OggStreamer _instance;
        private float[] _readBuffer;
        private short[] _castBuffer;
        internal HashSet<OggStream> _streams;
        private TimeSpan[] _threadTiming;
        private float _updateRate;

        private readonly Thread _thread;
        private bool _pendingFinish;
        private volatile bool _cancelled;

        public int BufferSize { get; }
        public TimeSpan UpdateDelay { get; private set; }

        public ReadOnlyMemory<TimeSpan> UpdateTiming => _threadTiming.AsMemory();

        public float UpdateRate
        {
            get => _updateRate;
            set
            {
                _updateRate = value;
                UpdateDelay = TimeSpan.FromSeconds(1 / ((value <= 0) ? 1.0 : value));
            }
        }

        public static OggStreamer Instance
        {
            get
            {
                lock (SingletonMutex)
                {
                    if (_instance == null)
                        throw new InvalidOperationException("No instance running.");
                    return _instance;
                }
            }
        }

        public OggStreamer(int bufferSize = DefaultBufferSize, float updateRate = DefaultUpdateRate)
        {
            lock (SingletonMutex)
            {
                if (_instance != null)
                    throw new InvalidOperationException("An instance has already been created.");
                _instance = this;
            }

            BufferSize = bufferSize;
            UpdateRate = updateRate;

            _threadTiming = new TimeSpan[(int)(UpdateRate < 1 ? 1 : UpdateRate)];
            
            _streams = new HashSet<OggStream>();

            _readBuffer = new float[BufferSize];
            if (!ALController.Instance.SupportsFloat32)
                _castBuffer = new short[BufferSize];

            _thread = new Thread(SongStreamingThread)
            {
                Name = "Song Streaming Thread",
                Priority = ThreadPriority.BelowNormal
            };
            _thread.Start();
        }

        public void Dispose()
        {
            lock (SingletonMutex)
            {
                Debug.Assert(_instance == this,
                    "A new instance was assigned without locking the singleton mutex.");

                _cancelled = true;
                lock (IterationMutex)
                    _streams.Clear();

                _thread.Join(1000);
                _instance = null;
            }
        }

        internal bool AddStream(OggStream stream)
        {
            lock (IterationMutex)
                return _streams.Add(stream);
        }

        internal bool RemoveStream(OggStream stream)
        {
            lock (IterationMutex)
                return _streams.Remove(stream);
        }

        public bool TryFillBuffer(OggStream stream, out ALBuffer buffer)
        {
            lock (FillMutex)
            {
                var reader = stream.Reader;
                int readSamples = reader.ReadSamples(_readBuffer);
                if (readSamples > 0)
                {
                    var channels = (AudioChannels)reader.Channels;
                    bool useFloat = ALController.Instance.SupportsFloat32;
                    ALFormat format = ALHelper.GetALFormat(channels, useFloat);

                    Span<float> dataSpan = _readBuffer.AsSpan(0, readSamples);
                    buffer = ALBufferPool.Rent();
                    if (useFloat)
                    {
                        buffer.BufferData(dataSpan, format, reader.SampleRate);
                    }
                    else
                    {
                        var castSpan = _castBuffer.AsSpan(0, readSamples);
                        AudioLoader.ConvertSingleToInt16(dataSpan, castSpan);
                        buffer.BufferData(castSpan, format, reader.SampleRate);
                    }
                    return true;
                }
            }
            buffer = null;
            return false;
        }

        private void SongStreamingThread()
        {
            var watch = new Stopwatch();
            var localStreams = new List<OggStream>();

            while (!_cancelled)
            {
                var sleepTime = UpdateDelay - watch.Elapsed;
                if (sleepTime.TotalMilliseconds > 0)
                    Thread.Sleep(sleepTime);

                if (_cancelled)
                    break;

                watch.Restart();

                localStreams.Clear();
                lock (IterationMutex)
                {
                    foreach (var stream in _streams)
                        localStreams.Add(stream);
                }

                foreach (OggStream stream in localStreams)
                {
                    lock (stream.ReadMutex)
                    {
                        lock (IterationMutex)
                            if (!_streams.Contains(stream))
                                continue;

                        if (!FillStream(stream))
                            continue;
                    }

                    lock (stream.StopMutex)
                    {
                        if (stream.IsPreparing)
                            continue;

                        lock (IterationMutex)
                            if (!_streams.Contains(stream))
                                continue;

                        var state = stream.GetState();
                        if (state == ALSourceState.Stopped)
                        {
                            AL.SourcePlay(stream.SourceId);
                            ALHelper.CheckError("Failed to play after fill.");
                        }
                    }
                }
                watch.Stop();

                // shift all elements forward and update the first element
                Array.Copy(_threadTiming, 0, _threadTiming, destinationIndex: 1, _threadTiming.Length - 1);
                _threadTiming[0] = watch.Elapsed;
            }
        }

        private bool FillStream(OggStream stream)
        {
            AL.GetSource(stream.SourceId, ALGetSourcei.BuffersProcessed, out int processed);
            ALHelper.CheckError("Failed to fetch processed buffers.");
            if (processed > 0)
            {
                AL.SourceUnqueueBuffers(stream.SourceId, processed);
                ALHelper.CheckError("Failed to unqueue buffers.");

                for (int i = 0; i < processed; i++)
                    stream.DequeueAndReturnBuffer();
            }

            int requested = Math.Max(MaxQueuedBuffers - stream.QueuedBufferCount, 0);
            if (requested == 0)
                return false;

            AL.GetSource(stream.SourceId, ALGetSourcei.BuffersQueued, out int queued);
            ALHelper.CheckError("Failed to fetch queued buffers.");

            int buffersFilled = 0;
            for (int i = 0; i < requested; i++)
            {
                if (TryFillBuffer(stream, out ALBuffer buffer))
                {
                    stream.QueueBuffer(buffer);
                    buffersFilled++;
                }
                else
                {
                    if (stream.IsLooped)
                    {
                        // closing and opening the stream is a bit expensive
                        //stream.Close();
                        //stream.Open();

                        // we don't support non-seekable streams anyway
                        stream.Reader.SamplePosition = 0;
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
                lock (IterationMutex)
                    _streams.Remove(stream);

                stream.OnFinished?.Invoke();
            }
            else if (buffersFilled > 0)
            {
                return true;
            }
            else if (!stream.IsLooped)
            {
                return false;
            }
            return true;
        }
    }
}