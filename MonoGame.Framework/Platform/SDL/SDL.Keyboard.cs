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
            [StructLayout(LayoutKind.Sequential)]
            public struct Keysym
            {
                public int Scancode;
                public int Sym;
                public KeyModifiers Mod;
                private readonly uint unused1;
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
            public delegate KeyModifiers d_SDL_GetModState();
            public static d_SDL_GetModState GetModState = 
                FL.LoadFunction<d_SDL_GetModState>(NativeLibrary, "SDL_GetModState");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_SDL_StartTextInput();
            public static d_SDL_StartTextInput StartTextInput = 
                FL.LoadFunction<d_SDL_StartTextInput>(NativeLibrary, "SDL_StartTextInput");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_SDL_StopTextInput();
            public static d_SDL_StopTextInput StopTextInput =
                FL.LoadFunction<d_SDL_StopTextInput>(NativeLibrary, "SDL_StopTextInput");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_SDL_SetTextInputRect(in Rect rect);
            public static d_SDL_SetTextInputRect SetTextInputRect =
                FL.LoadFunction<d_SDL_SetTextInputRect>(NativeLibrary, "SDL_SetTextInputRect");
        }
    }
}