// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Audio
{
    /// <summary>
    /// Handles the buffer events of all DynamicSoundEffectInstance instances.
    /// </summary>
    internal static class DynamicSoundEffectInstanceManager
    {
        private static List<WeakReference<DynamicSoundEffectInstance>> PlayingInstances { get; } =
            new List<WeakReference<DynamicSoundEffectInstance>>();

        public static void AddInstance(DynamicSoundEffectInstance instance)
        {
            var weakRef = new WeakReference<DynamicSoundEffectInstance>(instance);
            PlayingInstances.Add(weakRef);
        }

        public static void RemoveInstance(DynamicSoundEffectInstance instance)
        {
            for (int i = PlayingInstances.Count; i-- > 0;)
            {
                if (PlayingInstances[i].TryGetTarget(out var target) &&
                    target == instance)
                {
                    PlayingInstances.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Updates buffer queues of the currently playing instances.
        /// </summary>
        /// <remarks>
        /// XNA always posts <see cref="DynamicSoundEffectInstance.BufferNeeded"/> events on the main thread.
        /// </remarks>
        public static void Update()
        {
            for (int i = PlayingInstances.Count; i-- > 0;)
            {
                if (!PlayingInstances[i].TryGetTarget(out var target) ||
                    target.IsDisposed)
                {
                    PlayingInstances.RemoveAt(i);
                    continue;
                }

                target.UpdateQueue();
            }
        }
    }
}
