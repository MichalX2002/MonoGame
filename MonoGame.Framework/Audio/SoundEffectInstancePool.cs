// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Audio
{
    internal static class SoundEffectInstancePool
    {
        private static readonly object _syncRoot;

        private static readonly List<SoundEffectInstance> _playingInstances;
        private static readonly List<SoundEffectInstance> _pooledInstances;

        static SoundEffectInstancePool()
        {
            _syncRoot = new object();

            // Reduce garbage generation by allocating enough capacity for
            // the maximum playing instances or at least some reasonable value.
            int maxInstances = Math.Min(SoundEffect.MAX_PLAYING_INSTANCES, 1024);
            _playingInstances = new List<SoundEffectInstance>(maxInstances);
            _pooledInstances = new List<SoundEffectInstance>(maxInstances);
        }

        /// <summary>
        /// Gets a value indicating whether the platform has capacity for more sounds to be played at this time.
        /// </summary>
        /// <value><see langword="true"/> if more sounds can be played; otherwise, <see langword="false"/>.</value>
        internal static bool SlotsAvailable
        {
            get
            {
                lock (_syncRoot)
                    return _playingInstances.Count < SoundEffect.MAX_PLAYING_INSTANCES;
            }
        }

        /// <summary>
        /// Add the specified instance to the pool if it is a pooled instance and removes it from the
        /// list of playing instances.
        /// </summary>
        internal static void Return(SoundEffectInstance inst)
        {
            lock (_syncRoot)
            {
                if (inst._isPooled)
                {
                    _pooledInstances.Add(inst);
                    inst._effect = null;
                }

                _playingInstances.Remove(inst);
            }
        }

        /// <summary>
        /// Adds the <see cref="SoundEffectInstance"/> to the list of playing instances.
        /// </summary>
        internal static void AddToPlaying(SoundEffectInstance inst)
        {
            lock (_syncRoot)
                _playingInstances.Add(inst);
        }

        /// <summary>
        /// Returns a pooled <see cref="SoundEffectInstance"/> if one is available, or allocates a new
        /// <see cref="SoundEffectInstance"/> if the pool is empty.
        /// </summary>
        internal static SoundEffectInstance GetInstance(bool forXAct)
        {
            lock (_syncRoot)
            {
                SoundEffectInstance instance;

                int count = _pooledInstances.Count;
                if (count > 0)
                {
                    // Grab the item at the end of the list so the remove doesn't copy all
                    // the list items down one slot.
                    instance = _pooledInstances[count - 1];
                    _pooledInstances.RemoveAt(count - 1);

                    // Reset used instance to the "default" state.
                    instance._isPooled = true;
                    instance._isXAct = forXAct;
                    instance.Volume = 1f;
                    instance.Pan = 0f;
                    instance.Pitch = 0f;
                    instance.IsLooped = false;
                    instance.PlatformSetReverbMix(0);
                    instance.PlatformClearFilter();
                }
                else
                {
                    instance = new SoundEffectInstance
                    {
                        _isPooled = true,
                        _isXAct = forXAct
                    };
                }

                return instance;
            }
        }

        /// <summary>
        /// Iterates the list of playing instances, returning them to the pool if they
        /// have stopped playing.
        /// </summary>
        internal static void Update()
        {
            lock (_syncRoot)
            {
                // Cleanup instances which have finished playing.                    
                for (int i = 0; i < _playingInstances.Count; i++)
                {
                    var instance = _playingInstances[i];

                    // Don't consume XACT instances... XACT will
                    // clear this flag when it is done with the wave.
                    if (instance._isXAct)
                    {
                        i++;
                        continue;
                    }

                    if (instance.IsDisposed ||
                        instance.State == SoundState.Stopped ||
                        (instance._effect == null && !instance._isDynamic))
                    {
                        if (!instance.IsDisposed)
                            instance.Stop(true);

                        Return(instance);
                        continue;
                    }

                    i++;
                }

            }
        }

        internal static void DisposeInstances()
        {
            lock (_syncRoot)
            {
                foreach (var instance in _playingInstances)
                    instance.Dispose();
                _playingInstances.Clear();

                foreach (var instance in _pooledInstances)
                    instance.Dispose();
                _pooledInstances.Clear();
            }
        }

        /// <summary>
        /// Iterates the list of playing instances, stop them and return them to the pool if they are instances of the given SoundEffect.
        /// </summary>
        /// <param name="effect">The SoundEffect</param>
        internal static void StopPooledInstances(SoundEffect effect)
        {
            lock (_syncRoot)
            {
                for (int i = 0; i < _playingInstances.Count;)
                {
                    var instance = _playingInstances[i];
                    if (instance._effect == effect)
                    {
                        instance.Stop(true); // stop immediatly
                        Return(instance);
                        continue;
                    }
                    i++;
                }
            }
        }

        internal static void UpdateMasterVolume()
        {
            lock (_syncRoot)
            {
                foreach (var instance in _playingInstances)
                {
                    // XAct sounds are not controlled by the SoundEffect
                    // master volume, so we can skip them completely.
                    if (instance._isXAct)
                        continue;

                    // Re-applying the volume to itself will update
                    // the sound with the current master volume.
                    instance.Volume = instance.Volume;
                }
            }

        }
    }
}