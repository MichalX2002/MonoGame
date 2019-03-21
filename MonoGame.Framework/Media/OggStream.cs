// This code originated from:
//
//    http://theinstructionlimit.com/ogg-streaming-using-opentk-and-nvorbis
//    https://github.com/renaudbedard/nvorbis/
//
// It was released to the public domain by the author (Renaud Bedard).
// No other license is intended or required. 

using System;
using System.Collections.Generic;
using System.IO;
using NVorbis;
using MonoGame.OpenAL;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Media
{
    internal class OggStream : IDisposable
    {
        internal readonly object _stopMutex = new object();
        internal readonly object _prepareMutex = new object();

        internal readonly int _alSourceId;
        private readonly int _alFilterId;
        internal readonly Queue<ALSoundBuffer> _buffers;

        float _lowPassHfGain;
        float _volume;
        float _pitch;
        
        internal VorbisReader Reader { get; private set; }
        internal bool IsReady { get; private set; }
        internal bool IsPreparing { get; private set; }
        public Action OnFinished { get; private set; }
        
        public OggStream(string fileName, Action onFinished = null)
        {
            OnFinished = onFinished;

            _alSourceId = ALSoundController.Instance.ReserveSource();
            Reader = new VorbisReader(File.OpenRead(fileName), true);

            Volume = 1;
            Pitch = 1;

            if (OggStreamer.Instance.Efx.IsInitialized)
            {
                _alFilterId = OggStreamer.Instance.Efx.GenFilter();
                ALHelper.CheckError("Failed to generate Efx filter.");

                OggStreamer.Instance.Efx.Filter(_alFilterId, EfxFilteri.FilterType, (int)EfxFilterType.Lowpass);
                ALHelper.CheckError("Failed to set Efx filter type.");

                OggStreamer.Instance.Efx.Filter(_alFilterId, EfxFilterf.LowpassGain, 1);
                ALHelper.CheckError("Failed to set Efx filter value.");
                LowPassHFGain = 1;
            }
        }

        public void Prepare()
        {
            if (IsPreparing)
                return;

            var state = AL.GetSourceState(_alSourceId);
            ALHelper.CheckError("Failed to get source state.");

            lock (_stopMutex)
            {
                switch (state)
                {
                    case ALSourceState.Playing:
                    case ALSourceState.Paused:
                        return;

                    case ALSourceState.Stopped:
                        lock (_prepareMutex)
                        {
                            Close();
                            Empty();
                        }
                        break;
                }

                if (!IsReady)
                {
                    lock (_prepareMutex)
                    {
                        IsPreparing = true;
                        Open(precache: true);
                    }
                }
                else
                {
                    Reader.DecodedPosition = 0;
                    FillOneBuffer();
                }
            }
        }

        public void Play()
        {
            var state = AL.GetSourceState(_alSourceId);
            ALHelper.CheckError("Failed to get source state.");

            switch (state)
            {
                case ALSourceState.Playing:
                    return;

                case ALSourceState.Paused:
                    Resume();
                    return;

                default:
                    Prepare();

                    AL.SourcePlay(_alSourceId);
                    ALHelper.CheckError("Failed to play source.");

                    IsPreparing = false;
                    OggStreamer.Instance.AddStream(this);
                    break;
            }
        }

        public void Pause()
        {
            var state = AL.GetSourceState(_alSourceId);
            ALHelper.CheckError("Failed to get source state.");
            if (state != ALSourceState.Playing)
                return;

            OggStreamer.Instance.RemoveStream(this);
            AL.SourcePause(_alSourceId);
            ALHelper.CheckError("Failed to pause source.");
        }

        public void Resume()
        {
            var state = AL.GetSourceState(_alSourceId);
            ALHelper.CheckError("Failed to get source state.");
            if (state != ALSourceState.Paused)
                return;

            OggStreamer.Instance.AddStream(this);
            AL.SourcePlay(_alSourceId);
            ALHelper.CheckError("Failed to play source.");
        }

        public void Stop()
        {
            var state = AL.GetSourceState(_alSourceId);
            ALHelper.CheckError("Failed to get source state.");

            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
                StopPlayback();

            lock (_stopMutex)
            {
                OggStreamer.Instance.RemoveStream(this);

                lock (_prepareMutex)
                {
                    if (state != ALSourceState.Initial)
                        Empty(); // force the queued buffers to be unqueued to avoid issues on Mac
                }
            }
            AL.Source(_alSourceId, ALSourcei.Buffer, 0);
            ALHelper.CheckError("Failed to free source from buffers.");
        }

        public void SeekToPosition(TimeSpan pos)
        {
            Reader.DecodedTime = pos;
            SeekStopSource();
        }

        public void SeekToPosition(long pos)
        {
            Reader.DecodedPosition = pos;
            SeekStopSource();
        }

        private void SeekStopSource()
        {
            AL.SourceStop(_alSourceId);
            ALHelper.CheckError("Failed to stop source.");
        }

        public TimeSpan GetPosition()
        {
            if (Reader == null)
                return TimeSpan.Zero;

            return Reader.DecodedTime;
        }

        public TimeSpan GetLength()
        {
            return Reader.TotalTime;
        }

        public float LowPassHFGain
        {
            get => _lowPassHfGain;
            set
            {
                if (OggStreamer.Instance.Efx.IsInitialized)
                {
                    OggStreamer.Instance.Efx.Filter(_alFilterId, EfxFilterf.LowpassGainHF, _lowPassHfGain = value);
                    ALHelper.CheckError("Failed to set Efx filter.");

                    OggStreamer.Instance.Efx.BindFilterToSource(_alSourceId, _alFilterId);
                    ALHelper.CheckError("Failed to bind Efx filter to source.");
                }
            }
        }

        public float Volume
        {
            get => _volume;
            set
            {
                AL.Source(_alSourceId, ALSourcef.Gain, _volume = value);
                ALHelper.CheckError("Failed to set volume.");
            }
        }

        public float Pitch
        {
            get => _pitch;
            set
            {
                AL.Source(_alSourceId, ALSourcef.Pitch, _pitch = value);
                ALHelper.CheckError("Failed to set pitch.");
            }
        }

        public bool IsLooped { get; set; }

        void StopPlayback()
        {
            AL.SourceStop(_alSourceId);
            ALHelper.CheckError("Failed to stop source.");
        }

        void Empty()
        {
            AL.GetSource(_alSourceId, ALGetSourcei.BuffersQueued, out int queued);
            ALHelper.CheckError("Failed to fetch queued buffers.");

            if (queued > 0)
            {
                try
                {
                    AL.SourceUnqueueBuffers(_alSourceId, queued);
                    ALHelper.CheckError("Failed to unqueue buffers (first attempt).");
                }
                catch (InvalidOperationException)
                {
                    // This is a bug in the OpenAL implementation
                    // Salvage what we can
                    AL.GetSource(_alSourceId, ALGetSourcei.BuffersProcessed, out int processed);
                    ALHelper.CheckError("Failed to fetch processed buffers.");
                    
                    if (processed > 0)
                    {
                        AL.SourceUnqueueBuffers(_alSourceId, processed);
                        ALHelper.CheckError("Failed to unqueue buffers (second attempt).");
                    }

                    // Try turning it off again?
                    AL.SourceStop(_alSourceId);
                    ALHelper.CheckError("Failed to stop source.");

                    Empty();
                }
            }
        }

        internal void Open(bool precache = false)
        {
            if (precache)
                FillOneBuffer();

            IsReady = true;
        }

        private void FillOneBuffer()
        {
            // Fill first buffer synchronously
            OggStreamer.Instance.FillBuffer(this, _alBufferIds[0]);
            AL.SourceQueueBuffer(_alSourceId, _alBufferIds[0]);
            ALHelper.CheckError("Failed to queue buffer.");
        }

        public override int GetHashCode()
        {
            return _alSourceId;
        }

        internal void Close()
        {
            if (Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }
            IsReady = false;
        }

        public void Dispose()
        {
            var state = AL.GetSourceState(_alSourceId);
            ALHelper.CheckError("Failed to get the source state.");

            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
                StopPlayback();

            lock (_prepareMutex)
            {
                OggStreamer.Instance.RemoveStream(this);

                if (state != ALSourceState.Initial)
                    Empty();

                Close();
            }

            AL.Source(_alSourceId, ALSourcei.Buffer, 0);
            ALHelper.CheckError("Failed to free source from buffers.");

            ALSoundController.Instance.RecycleSource(_alSourceId);

            AL.DeleteBuffers(_alBufferIds);
            ALHelper.CheckError("Failed to delete buffer.");

            if (OggStreamer.Instance.Efx.IsInitialized)
            {
                OggStreamer.Instance.Efx.DeleteFilter(_alFilterId);
                ALHelper.CheckError("Failed to delete EFX filter.");
            }
        }
    }
}
