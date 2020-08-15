// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Audio
{
    internal static class SoundEffectInstancePool
    {
        private static object SyncRoot { get; } = new object();

        private static readonly List<SoundEffectInstance> _playingInstances;
        private static readonly List<SoundEffectInstance> _pooledInstances;

        static SoundEffectInstancePool()
        {
            // Reduce garbage generation by allocating enough capacity for
            // the maximum playing instances or at least some reasonable value.
            int maxInstances = Math.Min(SoundEffect.MaxPlayingInstances, 1024);

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
                lock (SyncRoot)
                    return _playingInstances.Count < SoundEffect.MaxPlayingInstances;
            }
        }

        /// <summary>
        /// Return the specified instance to the pool if it is a pooled instance and 
        /// remove it from the list of playing instances.
        /// </summary>
        internal static void Return(SoundEffectInstance inst)
        {
            lock (SyncRoot)
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
        internal static void Register(SoundEffectInstance inst)
        {
            lock (SyncRoot)
                _playingInstances.Add(inst);
        }

        /// <summary>
        /// Returns a pooled <see cref="SoundEffectInstance"/> if one is available, or allocates a new
        /// <see cref="SoundEffectInstance"/> if the pool is empty.
        /// </summary>
        internal static SoundEffectInstance GetInstance(bool forXAct)
        {
            lock (SyncRoot)
            {
                int count = _pooledInstances.Count;
                if (count > 0)
                {
                    // Grab the item at the end of the list so the remove doesn't copy all
                    // the list items down one slot.
                    var instance = _pooledInstances[count - 1];
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

                    return instance;
                }
                else
                {
                    return new SoundEffectInstance
                    {
                        _isPooled = true,
                        _isXAct = forXAct
                    };
                }
            }
        }

        /// <summary>
        /// Updates playing instances, 
        /// returning them to the pool if they have stopped playing.
        /// </summary>
        internal static void Update()
        {
            lock (SyncRoot)
            {
                // Cleanup instances which have finished playing.                    
                for (int i = _playingInstances.Count; i-- > 0;)
                {
                    var instance = _playingInstances[i];

                    // Don't consume XACT instances... XACT will
                    // clear this flag when it is done with the wave.
                    if (instance._isXAct)
                        continue;

                    if (instance.IsDisposed ||
                        instance.State == SoundState.Stopped ||
                        (instance._effect == null && !instance._isDynamic))
                    {
                        if (!instance.IsDisposed)
                            instance.Stop(true);

                        Return(instance);
                    }
                }
            }
        }

        /// <summary>
        /// Stop playing instances and return them to the pool if they are instances of the given SoundEffect.
        /// </summary>
        /// <param name="effect">The parent SoundEffect.</param>
        internal static void StopPooledInstances(SoundEffect effect)
        {
            lock (SyncRoot)
            {
                for (int i = _playingInstances.Count; i-- > 0;)
                {
                    var instance = _playingInstances[i];
                    if (instance._effect == effect)
                    {
                        instance.Stop(true); // stop immediatly
                        Return(instance);
                    }
                }
            }
        }

        internal static void DisposeInstances()
        {
            lock (SyncRoot)
            {
                foreach (var instance in _playingInstances)
                    instance.Dispose();
                _playingInstances.Clear();

                foreach (var instance in _pooledInstances)
                    instance.Dispose();
                _pooledInstances.Clear();
            }
        }

        internal static void UpdateMasterVolume()
        {
            lock (SyncRoot)
            {
                foreach (var instance in _playingInstances)
                    instance.UpdateMasterVolume();
            }

        }
    }
}