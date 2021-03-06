// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;

namespace MonoGame.Framework.Audio
{
    /// <summary>
    /// Manages the playback of a sound or set of sounds.
    /// </summary>
    /// <remarks>
    /// <para>Cues are comprised of one or more sounds.</para>
    /// <para>Cues also define specific properties such as pitch or volume.</para>
    /// <para>Cues are referenced through <see cref="SoundBank"/> objects.</para>
    /// </remarks>
    public class Cue : IDisposable
    {
        private readonly AudioEngine _engine;
        private readonly XactSound[] _sounds;
        private readonly float[] _probs;

        private readonly RpcVariable[] _variables;

        private XactSound _currentSound;

        private bool _applied3D;
        private bool _played;

        /// <summary>
        /// This event is triggered when the <see cref="Cue"/> is disposed.
        /// </summary>
        public event Event<Cue>? Disposed;

        /// <summary>
        /// Gets the friendly name of the cue.
        /// </summary>
        /// <remarks>
        /// The friendly name is a value set from the designer.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Is true if the <see cref="Cue"/> has been disposed.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        public bool IsPrepared { get; internal set; }

        public bool IsCreated { get; internal set; }

        // TODO: Implement me! 
        public bool IsStopping => false;

        public bool IsPreparing => false;

        /// <summary>
        /// Gets whether or not the cue is currently paused.
        /// </summary>
        /// <remarks>
        /// IsPlaying and IsPaused both return true if a cue is paused while playing.
        /// </remarks>
        public bool IsPaused
        {
            get
            {
                if (_currentSound != null)
                    return _currentSound.IsPaused;
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the cue is currently playing.
        /// </summary>
        /// <remarks>
        /// IsPlaying and IsPaused both return true if a cue is paused while playing.
        /// </remarks>
        public bool IsPlaying
        {
            get
            {
                if (_currentSound != null)
                    return _currentSound.Playing;
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the cue is currently stopped.
        /// </summary>
        public bool IsStopped
        {
            get
            {
                if (_currentSound != null)
                    return _currentSound.Stopped;
                return !IsDisposed && !IsPrepared;
            }
        }

        internal Cue(AudioEngine engine, string cuename, XactSound sound)
        {
            _engine = engine;
            Name = cuename;
            _sounds = new XactSound[1];
            _sounds[0] = sound;
            _probs = new float[1];
            _probs[0] = 1f;
            _variables = engine.CreateCueVariables();
        }

        internal Cue(AudioEngine engine, string cuename, XactSound[] sounds, float[] probs)
        {
            _engine = engine;
            Name = cuename;
            _sounds = sounds;
            _probs = probs;
            _variables = engine.CreateCueVariables();
        }

        internal void Prepare()
        {
            IsDisposed = false;
            IsCreated = false;
            IsPrepared = true;
            _currentSound = null;
        }

        /// <summary>Pauses playback.</summary>
        public void Pause()
        {
            lock (_engine.UpdateLock)
            {
                if (_currentSound != null)
                    _currentSound.Pause();
            }
        }

        /// <summary>
        /// Requests playback of a prepared or preparing <see cref="Cue"/>.
        /// </summary>
        public void Play()
        {
            lock (_engine.UpdateLock)
            {
                if (!_engine.ActiveCues.Contains(this))
                    _engine.ActiveCues.Add(this);

                //TODO: Probabilities
                var index = XactHelpers.Random.Next(_sounds.Length);
                _currentSound = _sounds[index];

                var volume = UpdateRpcCurves();

                _currentSound.Play(volume, _engine);
            }

            _played = true;
            IsPrepared = false;
        }

        /// <summary>Resumes playback of a paused <see cref="Cue"/>.</summary>
        public void Resume()
        {
            lock (_engine.UpdateLock)
            {
                if (_currentSound != null)
                    _currentSound.Resume();
            }
        }

        /// <summary>Stops playback of a <see cref="Cue"/>.</summary>
        /// <param name="options">
        /// Specifies if the sound should play any pending 
        /// release phases or transitions before stopping.
        /// </param>
        public void Stop(AudioStopOptions options)
        {
            lock (_engine.UpdateLock)
            {
                _engine.ActiveCues.Remove(this);

                if (_currentSound != null)
                    _currentSound.Stop(options);
            }

            IsPrepared = false;
        }

        private int FindVariable(string name)
        {
            // Do a simple linear search... which is fast
            // for as little variables as most cues have.
            for (var i = 0; i < _variables.Length; i++)
                if (_variables[i].Name == name)
                    return i;

            return -1;
        }

        /// <summary>
        /// Sets the value of a cue-instance variable based on its friendly name.
        /// </summary>
        /// <param name="name">Friendly name of the variable to set.</param>
        /// <param name="value">Value to assign to the variable.</param>
        /// <remarks>The friendly name is a value set from the designer.</remarks>
        public void SetVariable(string name, float value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var i = FindVariable(name);
            if (i == -1 || !_variables[i].IsPublic)
                throw new IndexOutOfRangeException("The specified variable index is invalid.");

            _variables[i].SetValue(value);
        }

        /// <summary>Gets a cue-instance variable value based on its friendly name.</summary>
        /// <param name="name">Friendly name of the variable.</param>
        /// <returns>Value of the variable.</returns>
        /// <remarks>
        ///The friendly name is a value set by the designer.
        ///<para>
        /// Cue-instance variables are useful when multiple instantiations of a single cue
        /// (and its associated sounds) are required 
        /// (for example, a "car" cue where there may be more than one car at any given time). 
        /// While a global variable allows multiple audio elements to be controlled in unison,
        /// a cue instance variable grants discrete control of each instance of a cue, 
        /// even for each copy of the same cue.
        /// </para>
        /// </remarks>
        public float GetVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var i = FindVariable(name);
            if (i == -1 || !_variables[i].IsPublic)
                throw new IndexOutOfRangeException("The specified variable index is invalid.");

            return _variables[i].Value;
        }

        /// <summary>
        /// Updates the simulated 3D audio settings with
        /// an <see cref="AudioEmitter"/> and <see cref="AudioListener"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// You must call <see cref="Apply3D(AudioListener, AudioEmitter)"/> 
        /// at least once before <see cref="Play()"/> to be able to call it in the future.
        /// </exception>
        /// <remarks>
        /// Calling this method automatically converts the sound to monoaural and
        /// sets the speaker mix for any sound played by this cue to a value calculated with
        /// the listener's and emitter's positions.
        /// Any stereo information in the sound will be discarded.
        /// </remarks>
        public void Apply3D(AudioListener listener, AudioEmitter emitter)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            if (emitter == null)
                throw new ArgumentNullException(nameof(emitter));

            if (_played && !_applied3D)
                throw new InvalidOperationException(
                    "You must call Apply3D() on a Cue before calling Play() " +
                    "to be able to call Apply3D() after calling Play().");

            var direction = listener.Position - emitter.Position;

            lock (_engine.UpdateLock)
            {
                // Set the distance for falloff.
                var distance = direction.Length();
                var i = FindVariable("Distance");
                _variables[i].SetValue(distance);

                // Calculate the orientation.
                if (distance > 0f)
                    direction /= distance;

                var right = Vector3.Cross(listener.Up, listener.Forward);
                var slope = Vector3.Dot(direction, listener.Forward);
                var angle = MathHelper.ToDegrees(MathF.Acos(slope));
                var j = FindVariable("OrientationAngle");
                _variables[j].SetValue(angle);
                if (_currentSound != null)
                    _currentSound.SetCuePan(Vector3.Dot(direction, right));

                // Calculate doppler effect.
                var relativeVelocity = emitter.Velocity - listener.Velocity;
                relativeVelocity *= emitter.DopplerScale;
            }

            _applied3D = true;
        }

        internal void Update(float deltaTime)
        {
            if (_currentSound == null)
                return;

            _currentSound.Update(deltaTime);

            UpdateRpcCurves();
        }

        private float UpdateRpcCurves()
        {
            var volume = 1f;

            // Evaluate the runtime parameter controls.
            var rpcCurves = _currentSound.RpcCurves;
            if (rpcCurves.Length > 0)
            {
                var pitch = 0f;
                var reverbMix = 1f;
                float? filterFrequency = null;
                float? filterQFactor = null;

                for (var i = 0; i < rpcCurves.Length; i++)
                {
                    var rpcCurve = _engine.RpcCurves[rpcCurves[i]];

                    // Some curves are driven by global variables
                    // and others by cue instance variables.
                    float value;
                    if (rpcCurve.IsGlobal)
                        value = rpcCurve.Evaluate(_engine.GetGlobalVariable(rpcCurve.Variable));
                    else
                        value = rpcCurve.Evaluate(_variables[rpcCurve.Variable].Value);

                    // Process the final curve value based on the parameter type it is.
                    switch (rpcCurve.Parameter)
                    {
                        case RpcParameter.Volume:
                            volume *= XactHelpers.ParseVolumeFromDecibels(value / 100f);
                            break;

                        case RpcParameter.Pitch:
                            pitch += value / 1000f;
                            break;

                        case RpcParameter.ReverbSend:
                            reverbMix *= XactHelpers.ParseVolumeFromDecibels(value / 100f);
                            break;

                        case RpcParameter.FilterFrequency:
                            filterFrequency = value;
                            break;

                        case RpcParameter.FilterQFactor:
                            filterQFactor = value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(rpcCurve.Parameter));
                    }
                }

                pitch = MathHelper.Clamp(pitch, -1f, 1f);
                if (volume < 0f)
                    volume = 0f;

                _currentSound.UpdateState(
                    _engine, volume, pitch, reverbMix, filterFrequency, filterQFactor);
            }

            return volume;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                if (disposing)
                {
                    IsCreated = false;
                    IsPrepared = false;
                    Disposed?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Disposes the <see cref="Cue"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

