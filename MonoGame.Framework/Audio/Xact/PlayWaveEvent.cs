// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Numerics;

namespace MonoGame.Framework.Audio
{
    enum VariationType
    {
        Ordered,
        OrderedFromRandom,
        Random,
        RandomNoImmediateRepeats,
        Shuffle
    };

    class PlayWaveEvent : ClipEvent
    {
        private readonly SoundBank _soundBank;
        private readonly VariationType _variation;
        private readonly int _loopCount;
        private readonly bool _newWaveOnLoop;

        private readonly int[] _tracks;
        private readonly int[] _waveBanks;
        
        private readonly byte[] _weights;
        private readonly int _totalWeights;

        private float _trackVolume;
        private float _trackPitch;
        private float _trackFilterFrequency;
        private float _trackFilterQFactor;

        private float _clipVolume;
        private float _clipPitch;
        private float _clipReverbMix;

        private readonly Vector4? _filterVar;
        private readonly Vector2? _volumeVar;
        private readonly Vector2? _pitchVar;

        private int _wavIndex;
        private int _loopIndex;

        private SoundEffectInstance? _wave;
        private bool _streaming;

        public PlayWaveEvent(   
            XactClip clip, float timeStamp, float randomOffset, SoundBank soundBank,
            int[] waveBanks, int[] tracks, byte[] weights, int totalWeights,
            VariationType variation, Vector2? volumeVar, Vector2? pitchVar, Vector4? filterVar,
            int loopCount, bool newWaveOnLoop)
            : base(clip, timeStamp, randomOffset)
        {
            _soundBank = soundBank;
            _waveBanks = waveBanks;
            _tracks = tracks;
            _weights = weights;
            _totalWeights = totalWeights;
            _volumeVar = volumeVar;
            _pitchVar = pitchVar;
            _filterVar = filterVar;
            _wavIndex = -1;
            _loopIndex = 0;

            _trackVolume = 1f;
            _trackPitch = 0;
            _trackFilterFrequency = 0;
            _trackFilterQFactor = 0;

            _clipVolume = 1f;
            _clipPitch = 0;
            _clipReverbMix = 0;

            _variation = variation;
            _loopCount = loopCount;
            _newWaveOnLoop = newWaveOnLoop;
        }

        public override void Play() 
        {
            if (_wave != null)
            {
                if (_wave.State != SoundState.Stopped)
                    _wave.Stop();
                if (_streaming)
                    _wave.Dispose();
                else					
                    _wave._isXAct = false;					
                _wave = null;
            }

            Play(true);
        }

        private void Play(bool pickNewWav)
        {
            var trackCount = _tracks.Length;

            // Do we need to pick a new wav to play first?
            if (pickNewWav)
            {
                switch (_variation)
                {
                    case VariationType.Ordered:
                        _wavIndex = (_wavIndex + 1) % trackCount;
                        break;

                    case VariationType.OrderedFromRandom:
                        _wavIndex = (_wavIndex + 1) % trackCount;
                        break;

                    case VariationType.Random:
                        if (_weights == null || trackCount == 1)
                            _wavIndex = XactHelpers.Random.Next() % trackCount;
                        else
                        {
                            var sum = XactHelpers.Random.Next(_totalWeights);
                            for (var i=0; i < trackCount; i++)
                            {
                                sum -= _weights[i];
                                if (sum <= 0)
                                {
                                    _wavIndex = i;
                                    break;
                                }
                            }
                        }
                        break;

                    case VariationType.RandomNoImmediateRepeats:
                    {
                        if (_weights == null || trackCount == 1)
                            _wavIndex = XactHelpers.Random.Next() % trackCount;
                        else
                        {
                            var last = _wavIndex;
                            var sum = XactHelpers.Random.Next(_totalWeights);
                            for (var i=0; i < trackCount; i++)
                            {
                                sum -= _weights[i];
                                if (sum <= 0)
                                {
                                    _wavIndex = i;
                                    break;
                                }
                            }

                            if (_wavIndex == last)
                                _wavIndex = (_wavIndex + 1) % trackCount;
                        }
                        break;
                    }

                    case VariationType.Shuffle:
                        // TODO: Need some sort of deck implementation.
                        _wavIndex = XactHelpers.Random.Next() % trackCount;
                        break;
                };
            }

            _wave = _soundBank.GetSoundEffectInstance(_waveBanks[_wavIndex], _tracks[_wavIndex], out _streaming);
            if (_wave == null)
            {
                // We couldn't create a sound effect instance, most likely
                // because we've reached the sound pool limits.
                return;
            }

            // Do all the randoms before we play.
            if (_volumeVar.HasValue)
                _trackVolume = _volumeVar.Value.X + ((float)XactHelpers.Random.NextDouble() * _volumeVar.Value.Y);

            if (_pitchVar.HasValue)
                _trackPitch = _pitchVar.Value.X + ((float)XactHelpers.Random.NextDouble() * _pitchVar.Value.Y);

            if (_clip.FilterEnabled)
            {
                if (_filterVar.HasValue)
                {
                    _trackFilterFrequency = 
                        _filterVar.Value.X + ((float)XactHelpers.Random.NextDouble() * _filterVar.Value.Y);

                    _trackFilterQFactor = 
                        _filterVar.Value.Z + ((float)XactHelpers.Random.NextDouble() * _filterVar.Value.W);
                }
                else
                {
                    _trackFilterFrequency = _clip.FilterFrequency;
                    _trackFilterQFactor = _clip.FilterQ;                
                }
            }
 
            // This is a shortcut for infinite looping of a single track.
            _wave.IsLooped = _loopCount == 255 && trackCount == 1;

            // Update all the wave states then play.
            UpdateState();
            _wave.Play();
        }

        public override void Stop()
        {
            if (_wave != null)
            {
                _wave.Stop();
                if (_streaming)
                    _wave.Dispose();
                else
                    _wave._isXAct = false;				
                _wave = null;
            }
            _loopIndex = 0;
        }

        public override void Pause() 
        {
            if (_wave != null)
                _wave.Pause();
        }

        public override void Resume()
        {
            if (_wave != null && _wave.State == SoundState.Paused)
                _wave.Resume();
        }

        public override void SetTrackVolume(float volume)
        {
            _clipVolume = volume;
            if (_wave != null)
                _wave.Volume = _trackVolume * _clipVolume;
        }

        public override void SetTrackPan(float pan)
        {
            if (_wave != null)
                _wave.Pan = pan;
        }

        public override void SetState(
            float volume, float pitch, float reverbMix, float? filterFrequency, float? filterQFactor)
        {
            _clipVolume = volume;
            _clipPitch = pitch;
            _clipReverbMix = reverbMix;

            // The RPC filter overrides the randomized track filter.
            if (filterFrequency.HasValue)
                _trackFilterFrequency = filterFrequency.Value;
            if (filterQFactor.HasValue)
                _trackFilterQFactor = filterQFactor.Value;

            if (_wave != null)
                UpdateState();
        }

        private void UpdateState()
        {
            Debug.Assert(_wave != null);

            _wave.Volume = _trackVolume * _clipVolume;
            _wave.Pitch = _trackPitch + _clipPitch;

            if (_clip.UseReverb)
                _wave.PlatformSetReverbMix(_clipReverbMix);

            if (_clip.FilterEnabled)
                _wave.PlatformSetFilter(_clip.FilterMode, _trackFilterQFactor, _trackFilterFrequency);
        }

        public override void SetFade(float fadeInDuration, float fadeOutDuration)
        {
            // TODO
        }

        public override bool Update(float dt)
        {
            if (_wave != null && _wave.State == SoundState.Stopped)
            {
                // If we're not looping or reached our loop limit then we can stop.
                if (_loopCount == 0 || _loopIndex >= _loopCount)
                {
                    if (_streaming)
                        _wave.Dispose();
                    else
                        _wave._isXAct = false;						
                    _wave = null;
                    _loopIndex = 0;
                }
                else
                {
                    // Increment the loop count if it isn't infinite.
                    if (_loopCount != 255)
                        _loopIndex++;

                    // Play the next track.
                    Play(_newWaveOnLoop);
                }
            }

            return _wave != null;
        }
    }
}

