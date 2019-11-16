// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.OpenAL;

namespace MonoGame.Framework.Audio
{
    public partial class SoundEffectInstance : IDisposable
    {
		private SoundState SoundState = SoundState.Stopped;
		private bool _looped = false;
		private float _alVolume = 1f;

        internal ALController Controller { get; private set; }
        internal uint? SourceId { get; set; }

        private float _reverb = 0f;
        private bool _needsFilterUpdate = false;
        private EfxFilterType _filterType;
        private float _filterQ; // unused?
        private float _frequency;
        
        /// <summary>
        /// Gets the OpenAL sound controller, constructs the sound buffer, and sets up the event delegates for
        /// the reserved and recycled events.
        /// </summary>
        internal void InitializeSound()
        {
            Controller = ALController.Instance;
        }

        internal void FreeSource()
        {
            Controller.RecycleSource(SourceId.Value);
            SourceId = null;
            SoundState = SoundState.Stopped;
        }

        private void PlatformApply3D(AudioListener listener, AudioEmitter emitter)
        {
            if (!SourceId.HasValue)
                return;

            AL.GetListener(ALListener3f.Position, out float x, out float y, out float z);
            ALHelper.CheckError("Failed to get source position.");

            // get the emitter offset from origin
            Vector3 posOffset = emitter.Position - listener.Position;

            // set up orientation matrix
            var orientation = Matrix.CreateWorld(Vector3.Zero, listener.Forward, listener.Up);

            // set up our final position and velocity according to orientation of listener
            var finalPos = Vector3.Transform(new Vector3(x, y, z) + posOffset, orientation);
            var finalVel = Vector3.Transform(emitter.Velocity, orientation);

            // set the position based on relative positon
            AL.Source(SourceId.Value, ALSource3f.Position, finalPos.X, finalPos.Y, finalPos.Z);
            ALHelper.CheckError("Failed to set source position.");

            AL.Source(SourceId.Value, ALSource3f.Velocity, finalVel.X, finalVel.Y, finalVel.Z);
            ALHelper.CheckError("Failed to set source velocity.");

            AL.Source(SourceId.Value, ALSourcef.ReferenceDistance, SoundEffect.DistanceScale);
            ALHelper.CheckError("Failed to set source distance scale.");

            AL.DopplerFactor(SoundEffect.DopplerScale);
            ALHelper.CheckError("Failed to set Doppler scale.");
        }

        private void PlatformPause()
        {
            if (!SourceId.HasValue || SoundState != SoundState.Playing)
                return;

            AL.SourcePause(SourceId.Value);
            ALHelper.CheckError("Failed to pause source.");
            SoundState = SoundState.Paused;
        }

        private void PlatformPlay()
        {
            SourceId = Controller.ReserveSource();
            
            AL.Source(SourceId.Value, ALSourcei.Buffer, _effect.SoundBuffer.BufferId);
            ALHelper.CheckError("Failed to bind buffer to source.");

            // update the position, gain, looping, pitch, and distance model

            AL.Source(SourceId.Value, ALSourcei.SourceRelative, 1);
            ALHelper.CheckError("Failed set source relative.");
            
            AL.DistanceModel(ALDistanceModel.InverseDistanceClamped);
            ALHelper.CheckError("Failed set source distance.");
            
            AL.Source(SourceId.Value, ALSource3f.Position, _pan, 0f, 0f);
            ALHelper.CheckError("Failed to set source pan.");
            
            AL.Source(SourceId.Value, ALSource3f.Velocity, 0f, 0f, 0f);
            ALHelper.CheckError("Failed to set source pan.");
            
            AL.Source(SourceId.Value, ALSourcef.Gain, _alVolume);
            ALHelper.CheckError("Failed to set source volume.");
            
            AL.Source(SourceId.Value, ALSourceb.Looping, IsLooped);
            ALHelper.CheckError("Failed to set source loop state.");
            
            AL.Source(SourceId.Value, ALSourcef.Pitch, _pitch);
            ALHelper.CheckError("Failed to set source pitch.");

            ApplyReverb();
            ApplyFilter();

            AL.SourcePlay(SourceId.Value);
            ALHelper.CheckError("Failed to play source.");

            SoundState = SoundState.Playing;
        }

        private void PlatformResume()
        {
            if (!SourceId.HasValue)
                return;

            if (SoundState == SoundState.Paused)
            {
                AL.SourcePlay(SourceId.Value);
                ALHelper.CheckError("Failed to play source.");

                SoundState = SoundState.Playing;
            }
        }

        private void PlatformStop(bool immediate)
        {
            if (SourceId.HasValue && AL.IsSource(SourceId.Value))
            {
                AL.SourceStop(SourceId.Value);
                ALHelper.CheckError("Failed to stop source.");

                // Reset the SendFilter to 0 if we are NOT using reverb since sources are recycled
                if (Controller.Efx.IsAvailable)
                {
                    Controller.Efx.BindSourceToAuxiliarySlot(SourceId.Value, 0, 0, 0);
                    ALHelper.CheckError("Failed to unset reverb.");

                    AL.Source(SourceId.Value, ALSourcei.EfxDirectFilter, 0);
                    ALHelper.CheckError("Failed to unset filter.");
                }

                AL.Source(SourceId.Value, ALSourcei.Buffer, 0);
                ALHelper.CheckError("Failed to free source from buffer.");

                FreeSource();
            }
        }

        private void PlatformSetIsLooped(bool value)
        {
            _looped = value;
            if (SourceId.HasValue)
            {
                AL.Source(SourceId.Value, ALSourceb.Looping, _looped);
                ALHelper.CheckError("Failed to set source loop state.");
            }
        }

        private bool PlatformGetIsLooped()
        {
            return _looped;
        }

        private void PlatformSetPan(float value)
        {
            if (SourceId.HasValue)
            {
                AL.Source(SourceId.Value, ALSource3f.Position, value, 0f, 0.1f);
                ALHelper.CheckError("Failed to set source pan.");
            }
        }

        private void PlatformSetPitch(float value)
        {
            if (SourceId.HasValue)
            {
                AL.Source(SourceId.Value, ALSourcef.Pitch, value);
                ALHelper.CheckError("Failed to set source pitch.");
            }
        }

        private SoundState PlatformGetState()
        {
            if (!SourceId.HasValue)
                return SoundState.Stopped;
            
            var alState = AL.GetSourceState(SourceId.Value);
            ALHelper.CheckError("Failed to get source state.");

            switch (alState)
            {
                case ALSourceState.Initial:
                case ALSourceState.Stopped:
                    SoundState = SoundState.Stopped;
                    break;

                case ALSourceState.Paused:
                    SoundState = SoundState.Paused;
                    break;

                case ALSourceState.Playing:
                    SoundState = SoundState.Playing;
                    break;
            }

            return SoundState;
        }

        private void PlatformSetVolume(float value)
        {
            _alVolume = value;
            if (SourceId.HasValue)
            {
                AL.Source(SourceId.Value, ALSourcef.Gain, _alVolume);
                ALHelper.CheckError("Failed to set source volume.");
            }
        }

        internal void PlatformSetReverbMix(float mix)
        {
            if (!Controller.Efx.IsAvailable)
                return;

            _reverb = mix;
            if (State == SoundState.Playing)
            {
                ApplyReverb();
                _reverb = 0f;
            }
        }

        private void ApplyReverb()
        {
            if (_reverb > 0f && SoundEffect.ReverbSlot != 0)
            {
                Controller.Efx.BindSourceToAuxiliarySlot(SourceId.Value, (int)SoundEffect.ReverbSlot, 0, 0);
                ALHelper.CheckError("Failed to set reverb.");
            }
        }

        private void ApplyFilter()
        {
            if (_needsFilterUpdate && Controller.Filter > 0)
            {
                float freq = _frequency / 20000f;
                float lf = 1f - freq;
                var efx = Controller.Efx;

                efx.Filter(Controller.Filter, EfxFilteri.FilterType, (int)_filterType);
                ALHelper.CheckError("Failed to set filter.");

                switch (_filterType)
                {
                    case EfxFilterType.Lowpass:
                        efx.Filter(Controller.Filter, EfxFilterf.LowpassGainHF, freq);
                        ALHelper.CheckError("Failed to set LowpassGainHF.");
                        break;

                    case EfxFilterType.Highpass:
                        efx.Filter(Controller.Filter, EfxFilterf.HighpassGainLF, freq);
                        ALHelper.CheckError("Failed to set HighpassGainLF.");
                        break;

                    case EfxFilterType.Bandpass:
                        efx.Filter(Controller.Filter, EfxFilterf.BandpassGainHF, freq);
                        ALHelper.CheckError("Failed to set BandpassGainHF.");

                        efx.Filter(Controller.Filter, EfxFilterf.BandpassGainLF, lf);
                        ALHelper.CheckError("Failed to set BandpassGainLF.");
                        break;
                }

                AL.Source(SourceId.Value, ALSourcei.EfxDirectFilter, Controller.Filter);
                ALHelper.CheckError("Failed to set DirectFilter.");

                _needsFilterUpdate = false;
            }
        }

        internal void PlatformSetFilter(FilterMode mode, float filterQ, float frequency)
        {
            if (!Controller.Efx.IsAvailable)
                return;

            switch (mode)
            {
                case FilterMode.BandPass:
                    _filterType = EfxFilterType.Bandpass;
                    break;

                case FilterMode.LowPass:
                    _filterType = EfxFilterType.Lowpass;
                    break;

                case FilterMode.HighPass:
                    _filterType = EfxFilterType.Highpass;
                    break;
            }

            _filterQ = filterQ;
            _frequency = frequency;

            // so ApplyFilter() can be called in PlatformPlay() instead
            _needsFilterUpdate = true;

            if (State == SoundState.Playing)
                ApplyFilter();
        }

        internal void PlatformClearFilter()
        {
            if (!Controller.Efx.IsAvailable)
                return;
            _needsFilterUpdate = false;
        }

        private void PlatformDispose(bool disposing)
        {
        }
    }
}
