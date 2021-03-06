﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Input
{
    internal static class KeysHelper
    {
        private static HashSet<int> _map;

        static KeysHelper()
        {
            var allKeys = (Keys[])Enum.GetValues(typeof(Keys));
            _map = new HashSet<int>(allKeys.Length);
            foreach (var key in allKeys)
                _map.Add((int)key);
        }

        /// <summary>
        /// Checks if specified value is valid Key.
        /// </summary>
        /// <param name="value">Keys base value</param>
        /// <returns>Returns true if value is valid Key, false otherwise</returns>
        public static bool IsKey(int value)
        {
            return _map.Contains(value);
        }
    }
}
