// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Audio;

namespace MonoGame.Framework
{
    /// <summary>
    /// Helper class for processing internal framework events.
    /// </summary>
    /// <remarks>
    /// If you use <see cref="Game"/> class, <see cref="Update()"/> is called automatically.
    /// Otherwise you must call it as part of your game loop.
    /// </remarks>
    public static class FrameworkDispatcher
    {
        private static bool _initialized;

        /// <summary>
        /// Processes framework events.
        /// </summary>
        public static void Update()
        {
            if (!_initialized)
                Initialize();

            DoUpdate();
        }

        private static void DoUpdate()
        {
            DynamicSoundEffectInstanceManager.Update();
            SoundEffectInstancePool.Update();
            Microphone.UpdateMicrophones();
        }

        private static void Initialize()
        {
            InitializeSoundSystem();
            _initialized = true;
        }

        private static void InitializeSoundSystem()
        {
            try
            {
                SoundEffect.PlatformInitialize();
            }
            catch (Exception ex)
            {
                throw new AudioHardwareException("Failed to initialize audio.", ex);
            }
        }
    }
}

