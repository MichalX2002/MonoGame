﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content.Pipeline
{
    [ContentSerializerCollectionItemName("Data")]
    public sealed class OpaqueDataDictionary : NamedValueDictionary<object>
    {
        /// <summary>
        /// Get the value for the specified key
        /// </summary>
        /// <key>The key of the item to retrieve.</key>
        /// <defaultValue>The default value to return if the key does not exist.</defaultValue>
        /// <returns>The item cast as T, or the default value if the item is not present in the dictonary.</returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            if (TryGetValue(key, out object o))
                return (T)o;
            return defaultValue;
        }
    }
}
