// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Runtime.InteropServices;
using MonoGame.Framework.Input;
using FL = MonoGame.Framework.FuncLoader;

namespace MonoGame
{
    internal static partial class SDL
    {
        public static class Keyboard
        {
            public struct Keysym
            {
                public int Scancode;
                public int Sym;
                public KeyModifiers Mod;
                public uint Unicode;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Event
            {
                public EventType Type;
                public uint TimeStamp;
                public uint WindowId;
                public byte State;
                public byte Repeat;
                private readonly byte padding2;
                private readonly byte padding3;
                public Keysym Keysym;
            }

            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct TextEditingEvent
            {
                public const int TextSize = 32;

                public EventType Type;
                public uint Timestamp;
                public uint WindowId;
                public fixed byte Text[TextSize];
                public int Start;
                public int Length;
            }

            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct TextInputEvent
            {
                public const int TextSize = 32;

                public EventType Type;
                public uint Timestamp;
                public uint WindowId;
                public fixed byte Text[TextSize];
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate KeyModifiers d_sdl_getmodstate();
            public static d_sdl_getmodstate GetModState = 
                FL.LoadFunction<d_sdl_getmodstate>(NativeLibrary, "SDL_GetModState");
        }
    }
}