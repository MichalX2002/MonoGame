// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Runtime.InteropServices;
using MonoGame.Framework;
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
                public KeyModifier Mod;
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
                public EventType Type;
                public uint Timestamp;
                public uint WindowId;
                public fixed byte Text[32];
                public int Start;
                public int Length;
            }

            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct TextInputEvent
            {
                public EventType Type;
                public uint Timestamp;
                public uint WindowId;
                public fixed byte Text[32];
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate KeyModifier d_sdl_getmodstate();
            public static d_sdl_getmodstate GetModState = 
                FL.LoadFunction<d_sdl_getmodstate>(NativeLibrary, "SDL_GetModState");
        }
    }
}