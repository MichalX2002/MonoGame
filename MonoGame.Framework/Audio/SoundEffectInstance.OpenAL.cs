// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.OpenAL;

namespace Microsoft.Xna.Framework.Audio
{
    public partial class SoundEffectInstance : IDisposable
    {
		internal SoundState SoundState = SoundState.Stopped;
		private bool _looped = false;
		private float _alVolume = 1f;

		internal int SourceID;
        private float reverb = 0f;
        bool applyFilter = false;
        EfxFilterType filterType;
        float _filterQ;
        float _frequency;
        
        internal ALController controller;
        internal bool HasSourceID = false;
        
        /// <summary>
        /// Gets the OpenAL sound controller, constructs the sound buffer, and sets up the event delegates for
        /// the reserved and recycled events.
        /// </summary>
        internal void InitializeSound()
        {
            controller = ALController.Instance;
        }

        private void PlatformApply3D(AudioListener listener, AudioEmitter emitter)
        {
            if (!HasSourceID)
                return;

            AL.GetListener(ALListener3f.Position, out float x, out float y, out float z);
            ALHelper.CheckError("Failed to get source position.");

            // get the emitter offset from origin
            Vector3 posOffset = emitter.Position - listener.Position;

            // set up orientation matrix
            Matrix orientation = Matrix.CreateWorld(Vector3.Zero, listener.Forward, listener.Up);

            // set up our final position and velocity according to orientation of listener
            Vector3 finalPos = Vector3.Transform(new Vector3(x, y, z) + posOffset, orientation);
            Vector3 finalVel = Vector3.Transform(emitter.Velocity, orientation);

            // set the position based on relative positon
            AL.Source(SourceID, ALSource3f.Position, finalPos.X, finalPos.Y, finalPos.Z);
            ALHelper.CheckError("Failed to set source position.");

            AL.Source(SourceID, ALSource3f.Velocity, finalVel.X, finalVel.Y, finalVel.Z);
            ALHelper.CheckError("Failed to set source velocity.");

            AL.Source(SourceID, ALSourcef.ReferenceDistance, SoundEffect.DistanceScale);
            ALHelper.CheckError("Failed to set source distance scale.");

            AL.DopplerFactor(SoundEffect.DopplerScale);
            ALHelper.CheckError("Failed to set Doppler scale.");
        }

        private void PlatformPause()
        {
            if (!HasSourceID || SoundState != SoundState.Playing)
                return;

            AL.SourcePause(SourceID);
            ALHelper.CheckError("Failed to pause source.");
            SoundState = SoundState.Paused;
        }

        private void PlatformPlay()
        {
            SourceID = controller.ReserveSource();
            HasSourceID = true;
            
            AL.Source(SourceID, ALSourcei.Buffer, _effect.SoundBuffer.BufferID);
            ALHelper.CheckError("Failed to bind buffer to source.");

            // Send the position, gain, looping, pitch, and distance model to the OpenAL driver.
            if (!HasSourceID)
                return;

            AL.Source(SourceID, ALSourcei.SourceRelative, 1);
            ALHelper.CheckError("Failed set source relative.");
            
            AL.DistanceModel(ALDistanceModel.InverseDistanceClamped);
            ALHelper.CheckError("Failed set source distance.");
            
            AL.Source(SourceID, ALSource3f.Position, _pan, 0f, 0f);
            ALHelper.CheckError("Failed to set source pan.");
            
            AL.Source(SourceID, ALSource3f.Velocity, 0f, 0f, 0f);
            ALHelper.CheckError("Failed to set source pan.");
            
            AL.Source(SourceID, ALSourcef.Gain, _alVolume);
            ALHelper.CheckError("Failed to set source volume.");
            
            AL.Source(SourceID, ALSourceb.Looping, IsLooped);
            ALHelper.CheckError("Failed to set source loop state.");
            
            AL.Source(SourceID, ALSourcef.Pitch, _pitch);
            ALHelper.CheckError("Failed to set source pitch.");

            ApplyReverb();
            ApplyFilter();

            AL.SourcePlay(SourceID);
            ALHelper.CheckError("Failed to play source.");

            SoundState = SoundState.Playing;
        }

        private void PlatformResume()
        {
            if (!HasSourceID)
                return;

            if (SoundState == SoundState.Paused)
            {
                AL.SourcePlay(SourceID);
                ALHelper.CheckError("Failed to play source.");
                SoundState = SoundState.Playing;
            }
        }

        private void PlatformStop(bool immediate)
        {
            if (HasSourceID)
            {
                AL.SourceStop(SourceID);
                ALHelper.CheckError("Failed to stop source.");

                // Reset the SendFilter to 0 if we are NOT using reverb since sources are recycled
                if (controller.SupportsEfx)
                {
                    ALController.Efx.BindSourceToAuxiliarySlot(SourceID, 0, 0, 0);
                    ALHelper.CheckError("Failed to unset reverb.");

                    AL.Source(SourceID, ALSourcei.EfxDirectFilter, 0);
                    ALHelper.CheckError("Failed to unset filter.");
                }

                AL.Source(SourceID, ALSourcei.Buffer, 0);
                ALHelper.CheckError("Failed to free source from buffer.");

                controller.FreeSource(this);
            }
            SoundState = SoundState.Stopped;
        }

        private void PlatformSetIsLooped(bool value)
        {
            _looped = value;
            if (HasSourceID)
            {
                AL.Source(SourceID, ALSourceb.Looping, _looped);
                ALHelper.CheckError("Failed to set source loop state.");
            }
        }

        private bool PlatformGetIsLooped()
        {
            return _looped;
        }

        private void PlatformSetPan(float value)
        {
            if (HasSourceID)
            {
                AL.Source(SourceID, ALSource3f.Position, value, 0.0f, 0.1f);
                ALHelper.CheckError("Failed to set source pan.");
            }
        }

        private void PlatformSetPitch(float value)
        {
            if (HasSourceID)
            {
                AL.Source(SourceID, ALSourcef.Pitch, value);
                ALHelper.CheckError("Failed to set source pitch.");
            }
        }

        private SoundState PlatformGetState()
        {
            if (!HasSourceID)
                return SoundState.Stopped;
            
            var alState = AL.GetSourceState(SourceID);
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
            if (HasSourceID)
            {
                AL.Source(SourceID, ALSourcef.Gain, _alVolume);
                ALHelper.CheckError("Failed to set source volume.");
            }
        }

        internal void PlatformSetReverbMix(float mix)
        {
            if (!ALController.Efx.IsInitialized)
                return;

            reverb = mix;
            if (State == SoundState.Playing)
            {
                ApplyReverb();
                reverb = 0f;
            }
        }

        void ApplyReverb()
        {
            if (reverb > 0f && SoundEffect.ReverbSlot != 0)
            {
                ALController.Efx.BindSourceToAuxiliarySlot(SourceID, (int)SoundEffect.ReverbSlot, 0, 0);
                ALHelper.CheckError("Failed to set reverb.");
            }
        }

        void ApplyFilter()
        {
            if (applyFilter && controller.Filter > 0)
            {
                var freq = _frequency / 20000f;
                var lf = 1.0f - freq;
                var efx = ALController.Efx;

                efx.Filter(controller.Filter, EfxFilteri.FilterType, (int)filterType);
                ALHelper.CheckError("Failed to set filter.");

                switch (filterType)
                {
                    case EfxFilterType.Lowpass:
                        efx.Filter(controller.Filter, EfxFilterf.LowpassGainHF, freq);
                        ALHelper.CheckError("Failed to set LowpassGainHF.");
                        break;

                    case EfxFilterType.Highpass:
                        efx.Filter(controller.Filter, EfxFilterf.HighpassGainLF, freq);
                        ALHelper.CheckError("Failed to set HighpassGainLF.");
                        break;

                    case EfxFilterType.Bandpass:
                        efx.Filter(controller.Filter, EfxFilterf.BandpassGainHF, freq);
                        ALHelper.CheckError("Failed to set BandpassGainHF.");

                        efx.Filter(controller.Filter, EfxFilterf.BandpassGainLF, lf);
                        ALHelper.CheckError("Failed to set BandpassGainLF.");
                        break;
                }

                AL.Source(SourceID, ALSourcei.EfxDirectFilter, controller.Filter);
                ALHelper.CheckError("Failed to set DirectFilter.");
            }
        }

        internal void PlatformSetFilter(FilterMode mode, float filterQ, float frequency)
        {
            if (!ALController.Efx.IsInitialized)
                return;

            applyFilter = true;
            switch (mode)
            {
                case FilterMode.BandPass:
                    filterType = EfxFilterType.Bandpass;
                    break;

                case FilterMode.LowPass:
                    filterType = EfxFilterType.Lowpass;
                    break;

                case FilterMode.HighPass:
                    filterType = EfxFilterType.Highpass;
                    break;
            }

            _filterQ = filterQ;
            _frequency = frequency;
            if (State == SoundState.Playing)
            {
                ApplyFilter();
                applyFilter = false;
            }
        }

        internal void PlatformClearFilter()
        {
            if (!ALController.Efx.IsInitialized)
                return;
            applyFilter = false;
        }

        private void PlatformDispose(bool disposing)
        {
        }
    }
}
