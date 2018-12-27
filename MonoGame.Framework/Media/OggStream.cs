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
        const int DefaultBufferCount = 3;

        internal readonly object _stopMutex = new object();
        internal readonly object _prepareMutex = new object();

        internal readonly int _alSourceId;
        internal readonly int[] _alBufferIds;
        internal readonly List<SongPart> _parts;

        readonly int _alFilterId;
        readonly string _oggFileName;

        internal VorbisReader Reader { get; private set; }
        internal bool Ready { get; private set; }
        internal bool Preparing { get; private set; }

        public Action FinishedAction { get; private set; }
        public int BufferCount { get; private set; }
        
        public OggStream(string filename, Action finishedAction = null, int bufferCount = DefaultBufferCount)
        {
            _oggFileName = filename;
            FinishedAction = finishedAction;
            BufferCount = bufferCount;

            _alBufferIds = AL.GenBuffers(bufferCount);
            ALHelper.CheckError("Failed to generate buffers.");
            _alSourceId = OpenALSoundController.Instance.ReserveSource();

            if (OggStreamer.Instance.XRam.IsInitialized)
            {
                OggStreamer.Instance.XRam.SetBufferMode(BufferCount, ref _alBufferIds[0], XRamExtension.XRamStorage.Hardware);
                ALHelper.CheckError("Failed to activate Xram.");
            }

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

            _parts = new List<SongPart>();
        }

        public void Prepare()
        {
            if (Preparing)
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

                if (!Ready)
                {
                    lock (_prepareMutex)
                    {
                        Preparing = true;
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
                case ALSourceState.Playing: return;
                case ALSourceState.Paused:
                    Resume();
                    return;
            }

            Prepare();

            AL.SourcePlay(_alSourceId);
            ALHelper.CheckError("Failed to play source.");

            Preparing = false;
            OggStreamer.Instance.AddStream(this);
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

        float lowPassHfGain;
        public float LowPassHFGain
        {
            get => lowPassHfGain;
            set
            {
                if (OggStreamer.Instance.Efx.IsInitialized)
                {
                    OggStreamer.Instance.Efx.Filter(_alFilterId, EfxFilterf.LowpassGainHF, lowPassHfGain = value);
                    ALHelper.CheckError("Failed to set Efx filter.");

                    OggStreamer.Instance.Efx.BindFilterToSource(_alSourceId, _alFilterId);
                    ALHelper.CheckError("Failed to bind Efx filter to source.");
                }
            }
        }

        float _volume;
        public float Volume
        {
            get => _volume;
            set
            {
                AL.Source(_alSourceId, ALSourcef.Gain, _volume = value);
                ALHelper.CheckError("Failed to set volume.");
            }
        }

        float _pitch;
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

            for (int i = 0; i < _parts.Count; i++)
                SongPartPool.Return(_parts[i]);
            _parts.Clear();
        }

        internal void AddPart(float[] buffer, int count, int partBufferSize)
        {
            var part = SongPartPool.Rent(partBufferSize);
            part.SetData(buffer, count);
            _parts.Add(part);
        }

        internal void RemovePart(int index)
        {
            SongPartPool.Return(_parts[index]);
            _parts.RemoveAt(index);
        }

        internal void Open(bool precache = false)
        {
            if(Reader == null)
                Reader = new VorbisReader(File.OpenRead(_oggFileName), true);

            if (precache)
                FillOneBuffer();

            Ready = true;
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
            Ready = false;
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

            OpenALSoundController.Instance.RecycleSource(_alSourceId);

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
