﻿using System;
using System.Collections.Generic;

namespace Lidgren.Network.Language
{
    public interface ILibraryLanguage
    {
        string this[string key] { get; }

        IEnumerable<KeyValuePair<string, string>> Pairs { get; }

        string GetString(string key);
    }
}