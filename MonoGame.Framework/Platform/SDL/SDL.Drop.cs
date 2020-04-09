// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame
{
    internal static partial class SDL
    {
        public static class Drop
        {
            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct DropEvent
            {
                public EventType Type;
                public uint Timestamp;
                public IntPtr File;
                public uint WindowId;
            }
        }
    }
}