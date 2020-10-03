// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using MonoGame.Framework.Collections;

namespace MonoGame.Framework.Input
{
    public static partial class Keyboard
    {
        internal static readonly List<Keys> _keysDown = new List<Keys>();
        internal static KeyModifiers _modifiers;

        private static ReadOnlyList<Keys> _keysDownRO = _keysDown.AsReadOnlyList();

        private static KeyModifiers PlatformGetModifiers()
        {
            return _modifiers;
        }

        private static ReadOnlyList<Keys> PlatformGetKeysDown()
        {
            return _keysDownRO;
        }

        private static KeyboardState PlatformGetState()
        {
            return new KeyboardState(_keysDown, _modifiers);
        }
    }
}
