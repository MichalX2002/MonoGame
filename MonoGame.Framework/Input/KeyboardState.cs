// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Holds the state of keystrokes by a keyboard.
    /// </summary>
	public struct KeyboardState : IEquatable<KeyboardState>
    {
        public const int MaxKeysPerState = 8;

        #region Key Data

        // Array of 256 bits:
        uint keys0, keys1, keys2, keys3, keys4, keys5, keys6, keys7;

        public int Count => GetCount();

        bool InternalGetKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);

            uint element;
            switch (((int)key) >> 5)
            {
                case 0: element = keys0; break;
                case 1: element = keys1; break;
                case 2: element = keys2; break;
                case 3: element = keys3; break;
                case 4: element = keys4; break;
                case 5: element = keys5; break;
                case 6: element = keys6; break;
                case 7: element = keys7; break;
                default: element = 0; break;
            }

            return (element & mask) != 0;
        }

        internal void InternalSetKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);
            switch (((int)key) >> 5)
            {
                case 0: keys0 |= mask; break;
                case 1: keys1 |= mask; break;
                case 2: keys2 |= mask; break;
                case 3: keys3 |= mask; break;
                case 4: keys4 |= mask; break;
                case 5: keys5 |= mask; break;
                case 6: keys6 |= mask; break;
                case 7: keys7 |= mask; break;
            }
        }

        internal void InternalClearKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);
            switch (((int)key) >> 5)
            {
                case 0: keys0 &= ~mask; break;
                case 1: keys1 &= ~mask; break;
                case 2: keys2 &= ~mask; break;
                case 3: keys3 &= ~mask; break;
                case 4: keys4 &= ~mask; break;
                case 5: keys5 &= ~mask; break;
                case 6: keys6 &= ~mask; break;
                case 7: keys7 &= ~mask; break;
            }
        }

        internal void InternalClearAllKeys()
        {
            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;
        }

        #endregion

        private int GetCount()
        {
            uint count = CountBits(keys0) + CountBits(keys1) + CountBits(keys2) + CountBits(keys3)
                    + CountBits(keys4) + CountBits(keys5) + CountBits(keys6) + CountBits(keys7);
            return (int)count;
        }

        #region XNA Interface

        /// <summary>
        /// Gets the current state of the Caps Lock key.
        /// </summary>
        public bool CapsLock { get; private set; }

        /// <summary>
        /// Gets the current state of the Num Lock key.
        /// </summary>
        public bool NumLock { get; private set; }

        internal KeyboardState(List<Keys> keys, bool capsLock = false, bool numLock = false)
        {
            CapsLock = capsLock;
            NumLock = numLock;

            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;

            if (keys != null)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    InternalSetKey(keys[i]);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardState"/> class.
        /// </summary>
        /// <param name="keys">List of keys to be flagged as pressed on initialization.</param>
        /// <param name="capsLock">Caps Lock state.</param>
        /// <param name="numLock">Num Lock state.</param>
        public KeyboardState(Keys[] keys, bool capsLock = false, bool numLock = false)
        {
            CapsLock = capsLock;
            NumLock = numLock;

            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;

            if (keys != null)
                for (int i = 0; i < keys.Length; i++)
                    InternalSetKey(keys[i]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardState"/> class.
        /// </summary>
        /// <param name="keys">List of keys to be flagged as pressed on initialization.</param>
        public KeyboardState(params Keys[] keys)
        {
            CapsLock = false;
            NumLock = false;

            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;

            if (keys != null)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    InternalSetKey(keys[i]);
                }
            }
        }

        /// <summary>
        /// Returns the state of a specified key.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>The state of the key.</returns>
        public KeyState this[Keys key]
        {
            get { return InternalGetKey(key) ? KeyState.Down : KeyState.Up; }
        }

        /// <summary>
        /// Gets whether given key is currently being pressed.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>true if the key is pressed; false otherwise.</returns>
        public bool IsKeyDown(Keys key)
        {
            return InternalGetKey(key);
        }

        /// <summary>
        /// Gets whether given key is currently being not pressed.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>true if the key is not pressed; false otherwise.</returns>
        public bool IsKeyUp(Keys key)
        {
            return !InternalGetKey(key);
        }

        #endregion


        #region GetPressedKeys()

        /// <summary>
        /// Returns the number of pressed keys in this <see cref="KeyboardState"/>.
        /// </summary>
        /// <returns>An integer representing the number of keys currently pressed in this <see cref="KeyboardState"/>.</returns>
        public int GetPressedKeyCount()
        {
            uint count = CountBits(keys0) + CountBits(keys1) + CountBits(keys2) + CountBits(keys3)
                    + CountBits(keys4) + CountBits(keys5) + CountBits(keys6) + CountBits(keys7);
            return (int)count;
        }

        private static uint CountBits(uint v)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            v -= (v >> 1) & 0x55555555;                    // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);     // temp
            return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
        }

        private static int AddKeysToArray(uint keys, int offset, Span<Keys> pressedKeys, int index)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((keys & (1 << i)) != 0)
                    pressedKeys[index++] = (Keys)(offset + i);
            }
            return index;
        }

        /// <summary>
        /// Returns an array of values keys that are currently being pressed.
        /// </summary>
        /// <returns>The keys that are currently being pressed.</returns>
        public Keys[] GetPressedKeys()
        {
            var keys = new Keys[GetCount()];
            GetPressedKeys(keys);
            return keys;
        }

        /// <summary>
        /// Fills an array of values holding keys that are currently being pressed.
        /// </summary>
        /// <param name="keys">
        /// The keys span to fill.
        /// This span is not cleared, and it must be equal to or larger than the number of keys pressed.
        /// </param>
        public int GetPressedKeys(Span<Keys> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            uint count = CountBits(keys0) + CountBits(keys1) + CountBits(keys2) + CountBits(keys3)
                    + CountBits(keys4) + CountBits(keys5) + CountBits(keys6) + CountBits(keys7);

            if (count > keys.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(keys),
                    "The supplied array cannot fit the number of pressed keys. " +
                    "Call GetPressedKeyCount() to get the number of pressed keys.");
            }

            int index = 0;
            if (keys0 != 0 && index < keys.Length) index = AddKeysToArray(keys0, 0 * 32, keys, index);
            if (keys1 != 0 && index < keys.Length) index = AddKeysToArray(keys1, 1 * 32, keys, index);
            if (keys2 != 0 && index < keys.Length) index = AddKeysToArray(keys2, 2 * 32, keys, index);
            if (keys3 != 0 && index < keys.Length) index = AddKeysToArray(keys3, 3 * 32, keys, index);
            if (keys4 != 0 && index < keys.Length) index = AddKeysToArray(keys4, 4 * 32, keys, index);
            if (keys5 != 0 && index < keys.Length) index = AddKeysToArray(keys5, 5 * 32, keys, index);
            if (keys6 != 0 && index < keys.Length) index = AddKeysToArray(keys6, 6 * 32, keys, index);
            if (keys7 != 0 && index < keys.Length) index = AddKeysToArray(keys7, 7 * 32, keys, index);

            return (int)count;
        }

        #endregion

        /// <summary>
        /// Returns the hash code of the <see cref="KeyboardState"/>.
        /// </summary>
        public override int GetHashCode()
        {
            return (int)(keys0 ^ keys1 ^ keys2 ^ keys3 ^ keys4 ^ keys5 ^ keys6 ^ keys7);
        }

        #region Equals

        public static bool operator ==(in KeyboardState a, in KeyboardState b)
        {
            return a.keys0 == b.keys0
                && a.keys1 == b.keys1
                && a.keys2 == b.keys2
                && a.keys3 == b.keys3
                && a.keys4 == b.keys4
                && a.keys5 == b.keys5
                && a.keys6 == b.keys6
                && a.keys7 == b.keys7;
        }

        public static bool operator !=(in KeyboardState a, in KeyboardState b) => !(a == b);

        public bool Equals(KeyboardState other) => this == other;
        public override bool Equals(object obj) => obj is KeyboardState other && Equals(other);

        #endregion
    }
}
