// This code originated from:
//
//    http://theinstructionlimit.com/ogg-streaming-using-opentk-and-nvorbis
//    https://github.com/renaudbedard/nvorbis/
//
// It was released to the public domain by the author (Renaud Bedard).
// No other license is intended or required. 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NVorbis;
using MonoGame.OpenAL;

namespace Microsoft.Xna.Framework.Audio
{
    internal class SongPart
    {
        public readonly float[] Data;
        public int Count;

        public SongPart(int size)
        {
            Data = new float[size];
        }

        public void SetData(float[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
                Data[i] = buffer[i];
            Count = count;
        }
    }

    internal class OggStream : IDisposable
    {
        const int DefaultBufferCount = 3;

        internal readonly object stopMutex = new object();
        internal readonly object prepareMutex = new object();

        internal readonly int alSourceId;
        internal readonly int[] alBufferIds;
        internal readonly List<SongPart> parts;

        readonly int alFilterId;
        readonly string oggFileName;

        internal VorbisReader Reader { get; private set; }
        internal bool Ready { get; private set; }
        internal bool Preparing { get; private set; }

        public Action FinishedAction { get; private set; }
        public int BufferCount { get; private set; }
        
        public OggStream(string filename, Action finishedAction = null, int bufferCount = DefaultBufferCount)
        {
            oggFileName = filename;
            FinishedAction = finishedAction;
            BufferCount = bufferCount;

            alBufferIds = AL.GenBuffers(bufferCount);
            ALHelper.CheckError("Failed to generate buffers.");
            alSourceId = OpenALSoundController.Instance.ReserveSource();

            if (OggStreamer.Instance.XRam.IsInitialized)
            {
                OggStreamer.Instance.XRam.SetBufferMode(BufferCount, ref alBufferIds[0], XRamExtension.XRamStorage.Hardware);
                ALHelper.CheckError("Failed to activate Xram.");
            }

            Volume = 1;
            Pitch = 1;

            if (OggStreamer.Instance.Efx.IsInitialized)
            {
                alFilterId = OggStreamer.Instance.Efx.GenFilter();
                ALHelper.CheckError("Failed to generate Efx filter.");
                OggStreamer.Instance.Efx.Filter(alFilterId, EfxFilteri.FilterType, (int)EfxFilterType.Lowpass);
                ALHelper.CheckError("Failed to set Efx filter type.");
                OggStreamer.Instance.Efx.Filter(alFilterId, EfxFilterf.LowpassGain, 1);
                ALHelper.CheckError("Failed to set Efx filter value.");
                LowPassHFGain = 1;
            }

            parts = new List<SongPart>();
        }

        public void Prepare()
        {
            if (Preparing)
                return;

            var state = AL.GetSourceState(alSourceId);
            ALHelper.CheckError("Failed to get source state.");

            lock (stopMutex)
            {
                switch (state)
                {
                    case ALSourceState.Playing:
                    case ALSourceState.Paused:
                        return;

                    case ALSourceState.Stopped:
                        lock (prepareMutex)
                        {
                            Close();
                            Empty();
                        }
                        break;
                }

                if (!Ready)
                {
                    lock (prepareMutex)
                    {
                        Preparing = true;
                        Open(precache: true);
                    }
                }
            }
        }

        public void Play()
        {
            var state = AL.GetSourceState(alSourceId);
            ALHelper.CheckError("Failed to get source state.");

            switch (state)
            {
                case ALSourceState.Playing: return;
                case ALSourceState.Paused:
                    Resume();
                    return;
            }

            Prepare();

            AL.SourcePlay(alSourceId);
            ALHelper.CheckError("Failed to play source.");

            Preparing = false;

            OggStreamer.Instance.AddStream(this);
        }

        public void Pause()
        {
            var state = AL.GetSourceState(alSourceId);
            ALHelper.CheckError("Failed to get source state.");
            if (state != ALSourceState.Playing)
                return;

            OggStreamer.Instance.RemoveStream(this);
            AL.SourcePause(alSourceId);
            ALHelper.CheckError("Failed to pause source.");
        }

        public void Resume()
        {
            var state = AL.GetSourceState(alSourceId);
            ALHelper.CheckError("Failed to get source state.");
            if (state != ALSourceState.Paused)
                return;

            OggStreamer.Instance.AddStream(this);
            AL.SourcePlay(alSourceId);
            ALHelper.CheckError("Failed to play source.");
        }

        public void Stop()
        {
            var state = AL.GetSourceState(alSourceId);
            ALHelper.CheckError("Failed to get source state.");
            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
                StopPlayback();

            lock (stopMutex)
            {
                OggStreamer.Instance.RemoveStream(this);

                lock (prepareMutex)
                {
                    if (state != ALSourceState.Initial)
                        Empty(); // force the queued buffers to be unqueued to avoid issues on Mac
                }
            }
            AL.Source(alSourceId, ALSourcei.Buffer, 0);
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
            AL.SourceStop(alSourceId);
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
            get { return lowPassHfGain; }
            set
            {
                if (OggStreamer.Instance.Efx.IsInitialized)
                {
                    OggStreamer.Instance.Efx.Filter(alFilterId, EfxFilterf.LowpassGainHF, lowPassHfGain = value);
                    ALHelper.CheckError("Failed to set Efx filter.");

                    OggStreamer.Instance.Efx.BindFilterToSource(alSourceId, alFilterId);
                    ALHelper.CheckError("Failed to bind Efx filter to source.");
                }
            }
        }

        float volume;
        public float Volume
        {
            get { return volume; }
            set
            {
                AL.Source(alSourceId, ALSourcef.Gain, volume = value);
                ALHelper.CheckError("Failed to set volume.");
            }
        }

        float pitch;
        public float Pitch
        {
            get { return pitch; }
            set
            {
                AL.Source(alSourceId, ALSourcef.Pitch, pitch = value);
                ALHelper.CheckError("Failed to set pitch.");
            }
        }

        public bool IsLooped { get; set; }

        public void Dispose()
        {
            var state = AL.GetSourceState(alSourceId);
            ALHelper.CheckError("Failed to get the source state.");
            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
                StopPlayback();

            lock (prepareMutex)
            {
                OggStreamer.Instance.RemoveStream(this);

                if (state != ALSourceState.Initial)
                    Empty();

                Close();
            }

            AL.Source(alSourceId, ALSourcei.Buffer, 0);
            ALHelper.CheckError("Failed to free source from buffers.");

            OpenALSoundController.Instance.RecycleSource(alSourceId);

            AL.DeleteBuffers(alBufferIds);
            ALHelper.CheckError("Failed to delete buffer.");

            if (OggStreamer.Instance.Efx.IsInitialized)
            {
                OggStreamer.Instance.Efx.DeleteFilter(alFilterId);
                ALHelper.CheckError("Failed to delete EFX filter.");
            }
        }

        void StopPlayback()
        {
            AL.SourceStop(alSourceId);
            ALHelper.CheckError("Failed to stop source.");
        }

        void Empty()
        {
            AL.GetSource(alSourceId, ALGetSourcei.BuffersQueued, out int queued);
            ALHelper.CheckError("Failed to fetch queued buffers.");

            if (queued > 0)
            {
                try
                {
                    AL.SourceUnqueueBuffers(alSourceId, queued);
                    ALHelper.CheckError("Failed to unqueue buffers (first attempt).");
                }
                catch (InvalidOperationException)
                {
                    // This is a bug in the OpenAL implementation
                    // Salvage what we can
                    AL.GetSource(alSourceId, ALGetSourcei.BuffersProcessed, out int processed);
                    ALHelper.CheckError("Failed to fetch processed buffers.");
                    
                    if (processed > 0)
                    {
                        AL.SourceUnqueueBuffers(alSourceId, processed);
                        ALHelper.CheckError("Failed to unqueue buffers (second attempt).");
                    }

                    // Try turning it off again?
                    AL.SourceStop(alSourceId);
                    ALHelper.CheckError("Failed to stop source.");

                    Empty();
                }
            }
        }

        internal void Open(bool precache = false)
        {
            Reader = new VorbisReader(File.OpenRead(oggFileName), true);

            if (precache)
            {
                // Fill first buffer synchronously
                OggStreamer.Instance.FillBuffer(this, alBufferIds[0]);
                AL.SourceQueueBuffer(alSourceId, alBufferIds[0]);
                ALHelper.CheckError("Failed to queue buffer.");
            }

            Ready = true;
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

        public override int GetHashCode()
        {
            return alSourceId;
        }
    }
}
