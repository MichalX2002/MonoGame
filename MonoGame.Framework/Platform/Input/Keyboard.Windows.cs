// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Framework.Collections;

namespace MonoGame.Framework.Input
{
    public static partial class Keyboard
    {
        private static KeyModifiers _modifiers;
        private static bool _isActive;

        private static Keys[] DefinedKeys { get; }

        private static List<Keys> KeysDownList { get; } = new List<Keys>();
        private static ReadOnlyList<Keys> KeysDownROList { get; } = KeysDownList.AsReadOnlyList();

        static Keyboard()
        {
            var definedValues = Enum.GetValues(typeof(Keys));
            var keyCodes = new List<Keys>(Math.Min(definedValues.Length, 255));
            foreach (var value in definedValues)
            {
                var key = (Keys)value!;
                if (((int)key >= 1) && ((int)key <= 255))
                    keyCodes.Add(key);
            }
            DefinedKeys = keyCodes.ToArray();
        }

        internal static void SetActive(bool isActive)
        {
            _isActive = isActive;

            if (!_isActive)
                KeysDownList.Clear();
        }

        private static KeyModifiers PlatformGetModifiers()
        {
            UpdateState(onlyModifiers: true);
            return _modifiers;
        }

        private static ReadOnlyList<Keys> PlatformGetKeysDown()
        {
            UpdateState(onlyModifiers: false);
            return KeysDownROList;
        }

        private static KeyboardState PlatformGetState()
        {
            UpdateState(onlyModifiers: false);
            return new KeyboardState(KeysDownList, _modifiers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsKeyPressed(Span<byte> state, Keys key)
        {
            return (state[(byte)key] & 0x80) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetModifierByState(Span<byte> state, Keys key, KeyModifiers modifier)
        {
            if (IsKeyPressed(state, key))
                _modifiers |= modifier;
        }

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(ref byte lpKeyState);

        [SkipLocalsInit]
        private static void UpdateState(bool onlyModifiers)
        {
            _modifiers = KeyModifiers.None;

            if (Console.CapsLock)
                _modifiers |= KeyModifiers.CapsLock;

            if (Console.NumberLock)
                _modifiers |= KeyModifiers.NumLock;

            if (_isActive)
            {
                Span<byte> keyState = stackalloc byte[256];
                if (GetKeyboardState(ref MemoryMarshal.GetReference(keyState)))
                {
                    if (onlyModifiers)
                    {
                        SetModifierByState(keyState, Keys.LeftShift, KeyModifiers.LeftShift);
                        SetModifierByState(keyState, Keys.RightShift, KeyModifiers.RightShift);
                        SetModifierByState(keyState, Keys.LeftAlt, KeyModifiers.LeftAlt);
                        SetModifierByState(keyState, Keys.RightAlt, KeyModifiers.RightAlt);
                        SetModifierByState(keyState, Keys.LeftControl, KeyModifiers.LeftControl);
                        SetModifierByState(keyState, Keys.RightControl, KeyModifiers.RightControl);
                    }
                    else
                    {
                        foreach (Keys key in DefinedKeys)
                        {
                            if (IsKeyPressed(keyState, key))
                            {
                                switch (key)
                                {
                                    case Keys.LeftShift:
                                        _modifiers |= KeyModifiers.LeftShift;
                                        break;
                                    case Keys.RightShift:
                                        _modifiers |= KeyModifiers.RightShift;
                                        break;

                                    case Keys.LeftAlt:
                                        _modifiers |= KeyModifiers.LeftAlt;
                                        break;
                                    case Keys.RightAlt:
                                        _modifiers |= KeyModifiers.RightAlt;
                                        break;

                                    case Keys.LeftControl:
                                        _modifiers |= KeyModifiers.LeftControl;
                                        break;
                                    case Keys.RightControl:
                                        _modifiers |= KeyModifiers.RightControl;
                                        break;
                                }

                                if (!KeysDownList.Contains(key))
                                    KeysDownList.Add(key);
                            }
                            else
                            {
                                KeysDownList.Remove(key);
                            }
                        }
                    }
                }
            }
            else
            {
                KeysDownList.Clear();
            }
        }
    }
}
