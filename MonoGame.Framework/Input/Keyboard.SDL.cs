// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Input
{
    public static partial class Keyboard
    {
        static List<Keys> _keys;
        public static ReadOnlyCollection<Keys> KeyList { get; private set; }

        private static KeyboardState PlatformGetState()
        {
            var modifiers = Sdl.Keyboard.GetModState();
            return new KeyboardState(_keys,
                                     (modifiers & Sdl.Keyboard.Keymod.CapsLock) == Sdl.Keyboard.Keymod.CapsLock,
                                     (modifiers & Sdl.Keyboard.Keymod.NumLock) == Sdl.Keyboard.Keymod.NumLock);
        }

        internal static void SetKeys(List<Keys> keys)
        {
            _keys = keys;
            KeyList = _keys.AsReadOnly();
        }
    }
}
