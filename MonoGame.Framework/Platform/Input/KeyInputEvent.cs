// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
    public readonly struct KeyInputEvent
    {
        /// <summary>
        /// The key that was either pressed or released.
        /// </summary>
        public readonly Keys Key;

        /// <summary>
        /// Create a new keyboard input event.
        /// </summary>
        /// <param name="key">The key involved in this event</param>
        public KeyInputEvent(Keys key = Keys.None)
        {
            Key = key;
        }
    }
}
