// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Collections;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Allows getting keystrokes from keyboard.
    /// </summary>
    public static partial class Keyboard
    {
        /// <summary>
        /// Gets the currently active key modifiers.
        /// </summary>
        public static KeyModifiers Modifiers => PlatformGetModifiers();

        /// <summary>
        /// Gets all the currently pressed keys.
        /// </summary>
        public static ReadOnlyList<Keys> KeysDown => PlatformGetKeysDown();

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
