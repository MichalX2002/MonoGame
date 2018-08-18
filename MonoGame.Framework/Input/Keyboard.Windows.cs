﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Input
{
    public static partial class Keyboard
    {
        private static readonly byte[] DefinedKeyCodes;

        private static readonly byte[] _keyState = new byte[256];
        private static readonly List<Keys> _keys = new List<Keys>(10);
        private static readonly ReadOnlyCollection<Keys> _readOnlyKeys;
        private static KeyModifier _keyModifier;

        private static bool _isActive;

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        private static readonly Predicate<Keys> IsKeyReleasedPredicate = key => IsKeyReleased((byte)key);

        public static ReadOnlyCollection<Keys> KeyList { get { PlatformGetState(); return _readOnlyKeys; } }
        public static KeyModifier Modifiers { get { PlatformGetState(); return _keyModifier; } }

        static Keyboard()
        {
            var definedKeys = Enum.GetValues(typeof(Keys));
            var keyCodes = new List<byte>(Math.Min(definedKeys.Length, 255));
            foreach (var key in definedKeys)
            {
                var keyCode = (int)key;
                if ((keyCode >= 1) && (keyCode <= 255))
                    keyCodes.Add((byte)keyCode);
            }
            DefinedKeyCodes = keyCodes.ToArray();

            _readOnlyKeys = new ReadOnlyCollection<Keys>(_keys);
        }

        private static KeyboardState PlatformGetState()
        {
            if (_isActive && GetKeyboardState(_keyState))
            {
                _keys.RemoveAll(IsKeyReleasedPredicate);

                foreach (var keyCode in DefinedKeyCodes)
                {
                    if (IsKeyReleased(keyCode))
                        continue;

                    var key = (Keys)keyCode;
                    if (!_keys.Contains(key))
                        _keys.Add(key);
                }
            }

            _keyModifier = KeyModifier.None;
            if (Console.CapsLock)
                _keyModifier |= KeyModifier.CapsLock;
            if (Console.NumberLock)
                _keyModifier |= KeyModifier.NumLock;

            if(!IsKeyReleased((byte)Keys.LeftShift))
                _keyModifier |= KeyModifier.LeftShift;
            if(!IsKeyReleased((byte)Keys.RightShift))
                _keyModifier |= KeyModifier.RightShift;

            if (!IsKeyReleased((byte)Keys.LeftAlt))
                _keyModifier |= KeyModifier.LeftAlt;
            if (!IsKeyReleased((byte)Keys.RightAlt))
                _keyModifier |= KeyModifier.RightAlt;

            if (!IsKeyReleased((byte)Keys.LeftControl))
                _keyModifier |= KeyModifier.LeftCtrl;
            if (!IsKeyReleased((byte)Keys.RightControl))
                _keyModifier |= KeyModifier.RightCtrl;

            return new KeyboardState(_keys, Console.CapsLock, Console.NumberLock);
        }

        private static bool IsKeyReleased(byte keyCode)
        {
            return ((_keyState[keyCode] & 0x80) == 0);
        }

        internal static void SetActive(bool isActive)
        {
            _isActive = isActive;
            if (!_isActive)
                _keys.Clear();
        }
    }
}
