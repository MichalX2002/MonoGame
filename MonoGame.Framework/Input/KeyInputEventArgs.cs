// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
    /// <summary>
    /// Represents data for a keystroke event.
    /// </summary>
    public readonly struct KeyInputEventArgs
    {
        /// <summary>
        /// Gets the key that was either pressed or released.
        /// </summary>
        public Keys Key { get; }

        /// <summary>
        /// Constructs the <see cref="KeyInputEventArgs"/>.
        /// </summary>
        /// <param name="key">The key involved in this event</param>
        public KeyInputEventArgs(Keys key = Keys.None)
        {
            Key = key;
        }
    }
}
