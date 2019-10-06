// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace MonoGame.Framework.Audio
{
    /// <summary>
    /// Provides functionality for manipulating multiple sounds at a time.
    /// </summary>
    public struct AudioCategory : IEquatable<AudioCategory>
    {
        readonly AudioEngine _engine;
        readonly List<XactSound> _sounds;

        // This is a bit gross, but we use an array here
        // instead of a field since AudioCategory is a struct
        // This allows us to save _volume when the user
        // holds onto a reference of AudioCategory, or when a cue
        // is created/loaded after the volume's already been set.
        internal float[] _volume;

        internal bool isBackgroundMusic;
        internal bool isPublic;

        internal bool instanceLimit;
        internal int maxInstances;
        internal MaxInstanceBehavior InstanceBehavior;

        internal CrossfadeType fadeType;
        internal float fadeIn;
        internal float fadeOut;

        internal AudioCategory(AudioEngine audioengine, string name, BinaryReader reader)
        {
            Debug.Assert(audioengine != null);
            Debug.Assert(!string.IsNullOrEmpty(name));

            _sounds = new List<XactSound>();
            Name = name;
            _engine = audioengine;

            maxInstances = reader.ReadByte();
            instanceLimit = maxInstances != 0xff;

            fadeIn = reader.ReadUInt16() / 1000f;
            fadeOut = reader.ReadUInt16() / 1000f;

            byte instanceFlags = reader.ReadByte();
            fadeType = (CrossfadeType)(instanceFlags & 0x7);
            InstanceBehavior = (MaxInstanceBehavior)(instanceFlags >> 3);

            reader.ReadUInt16(); //unkn

            var volume = XactHelpers.ParseVolumeFromDecibels(reader.ReadByte());
            _volume = new float[1] { volume };

            byte visibilityFlags = reader.ReadByte();
            isBackgroundMusic = (visibilityFlags & 0x1) != 0;
            isPublic = (visibilityFlags & 0x2) != 0;
        }

        internal void AddSound(XactSound sound)
        {
            _sounds.Add(sound);
        }

        internal int GetPlayingInstanceCount()
        {
            int sum = 0;
            for (var i = 0; i < _sounds.Count; i++)
            {
                if (_sounds[i].Playing)
                    sum++;
            }
            return sum;
        }

        internal XactSound GetOldestInstance()
        {
            for (var i = 0; i < _sounds.Count; i++)
            {
                if (_sounds[i].Playing)
                    return _sounds[i];
            }
            return null;
        }

        /// <summary>
        /// Gets the category's friendly name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Pauses all associated sounds.
        /// </summary>
        public void Pause()
        {
            foreach (var sound in _sounds)
                sound.Pause();
        }

        /// <summary>
        /// Resumes all associated paused sounds.
        /// </summary>
        public void Resume()
        {
            foreach (var sound in _sounds)
                sound.Resume();
        }

        /// <summary>
        /// Stops all associated sounds.
        /// </summary>
        public void Stop(AudioStopOptions options)
        {
            foreach (var sound in _sounds)
                sound.Stop(options);
        }

        public void SetVolume(float volume)
        {
            if (volume < 0)
                throw new ArgumentException("The volume must be positive.");

            // Updating all the sounds in a category can be
            // very expensive... so avoid it if we can.
            if (_volume[0] == volume)
                return;

            _volume[0] = volume;

            foreach (var sound in _sounds)
                sound.UpdateCategoryVolume(volume);
        }

        /// <summary>
        /// Determines whether two <see cref="AudioCategory"/> instances are equal.
        /// </summary>
        public static bool operator ==(in AudioCategory a, in AudioCategory b) =>
            a._engine == b._engine &&
            a.Name.Equals(b.Name, StringComparison.Ordinal);

        /// <summary>
        /// Determines whether two <see cref="AudioCategory"/> instances are not equal.
        /// </summary>
        public static bool operator !=(in AudioCategory a, in AudioCategory b) => !(a == b);

        /// <summary>
        /// Determines whether two <see cref="AudioCategory"/> instances are equal.
        /// </summary>
        public bool Equals(AudioCategory other) => this == other;

        /// <summary>
        /// Determines whether two <see cref="AudioCategory"/> instances are equal.
        /// </summary>
        /// <param name="obj">Object to compare with this instance.</param>
        /// <returns>true if the objects are equal or false if they aren't.</returns>
        public override bool Equals(object obj) => obj is AudioCategory other && Equals(other);

        /// <summary>
        /// Returns the friendly name of this <see cref="AudioCategory"/>.
        /// </summary>
        public override string ToString() => Name;

        /// <summary>
        /// Gets the hash code for this <see cref="AudioCategory"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int code = 7 + Name.GetHashCode();
                return code * 31 + _engine.GetHashCode();
            }
        }
    }
}