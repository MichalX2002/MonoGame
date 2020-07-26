// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Allows getting keystrokes from keyboard.
    /// </summary>
    public static partial class Keyboard
    {
        /// <summary>
        /// Returns the current keyboard state.
        /// </summary>
        /// <returns>Snapshot of current keyboard state.</returns>
        public static KeyboardState GetState()
        {
            return PlatformGetState();
        }
    }
}
