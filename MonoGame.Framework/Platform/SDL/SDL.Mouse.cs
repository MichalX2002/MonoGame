// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using FL = MonoGame.Framework.FuncLoader;

namespace MonoGame
{
    internal static partial class SDL
    {
        public static class Mouse
        {
            [Flags]
            public enum Button
            {
                Left = 1 << 0,
                Middle = 1 << 1,
                Right = 1 << 2,
                X1Mask = 1 << 3,
                X2Mask = 1 << 4
            }

            public enum SystemCursor
            {
                Arrow,
                IBeam,
                Wait,
                Crosshair,
                WaitArrow,
                SizeNWSE,
                SizeNESW,
                SizeWE,
                SizeNS,
                SizeAll,
                No,
                Hand
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MotionEvent
            {
                public EventType Type;
                public uint Timestamp;
                public uint WindowId;
                public uint Which;
                public byte State;
                private readonly byte _padding1;
                private readonly byte _padding2;
                private readonly byte _padding3;
                public int X;
                public int Y;
                public int Xrel;
                public int Yrel;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WheelEvent
            {
                public EventType Type;
                public uint TimeStamp;
                public uint WindowId;
                public uint Which;
                public int X;
                public int Y;
                public uint Direction;
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_createcolorcursor(IntPtr surface, int x, int y);
            private static readonly d_sdl_createcolorcursor SDL_CreateColorCursor = 
                FL.LoadFunction<d_sdl_createcolorcursor>(NativeLibrary, "SDL_CreateColorCursor");

            public static IntPtr CreateColorCursor(IntPtr surface, int x, int y)
            {
                return GetError(SDL_CreateColorCursor(surface, x, y));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_createsystemcursor(SystemCursor id);
            private static readonly d_sdl_createsystemcursor SDL_CreateSystemCursor = 
                FL.LoadFunction<d_sdl_createsystemcursor>(NativeLibrary, "SDL_CreateSystemCursor");

            public static IntPtr CreateSystemCursor(SystemCursor id)
            {
                return GetError(SDL_CreateSystemCursor(id));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_freecursor(IntPtr cursor);
            public static d_sdl_freecursor FreeCursor = 
                FL.LoadFunction<d_sdl_freecursor>(NativeLibrary, "SDL_FreeCursor");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate Button d_sdl_getglobalmousestate(out int x, out int y);
            public static d_sdl_getglobalmousestate GetGlobalState = 
                FL.LoadFunction<d_sdl_getglobalmousestate>(NativeLibrary, "SDL_GetGlobalMouseState");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate Button d_sdl_getmousestate(out int x, out int y);
            public static d_sdl_getmousestate GetState =
                FL.LoadFunction<d_sdl_getmousestate>(NativeLibrary, "SDL_GetMouseState");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_setcursor(IntPtr cursor);
            public static d_sdl_setcursor SetCursor = 
                FL.LoadFunction<d_sdl_setcursor>(NativeLibrary, "SDL_SetCursor");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_showcursor(int toggle);
            public static d_sdl_showcursor ShowCursor =
                FL.LoadFunction<d_sdl_showcursor>(NativeLibrary, "SDL_ShowCursor");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_warpmouseinwindow(IntPtr window, int x, int y);
            public static d_sdl_warpmouseinwindow WarpInWindow = 
                FL.LoadFunction<d_sdl_warpmouseinwindow>(NativeLibrary, "SDL_WarpMouseInWindow");
        }
    }
}