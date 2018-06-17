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
        public static KeyModifier Modifiers { get; internal set; }

        private static KeyboardState PlatformGetState()
        {
            return new KeyboardState(_keys,
                                     (Modifiers & KeyModifier.CapsLock) == KeyModifier.CapsLock,
                                     (Modifiers & KeyModifier.NumLock) == KeyModifier.NumLock);
        }

        internal static void SetKeys(List<Keys> keys)
        {
            _keys = keys;
            KeyList = _keys.AsReadOnly();
        }
    }
}
