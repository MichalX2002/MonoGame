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
        public static class Display
        {
            public struct Mode
            {
                public uint Format;
                public int Width;
                public int Height;
                public int RefreshRate;
                public IntPtr DriverData;
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_getdisplaybounds(int displayIndex, out Rectangle rect);
            private static readonly d_sdl_getdisplaybounds SDL_GetDisplayBounds = 
                FL.LoadFunction<d_sdl_getdisplaybounds>(NativeLibrary, "SDL_GetDisplayBounds");

            public static void GetBounds(int displayIndex, out Rectangle rect)
            {
                GetError(SDL_GetDisplayBounds(displayIndex, out rect));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_getcurrentdisplaymode(int displayIndex, out Mode mode);
            private static readonly d_sdl_getcurrentdisplaymode SDL_GetCurrentDisplayMode = 
                FL.LoadFunction<d_sdl_getcurrentdisplaymode>(NativeLibrary, "SDL_GetCurrentDisplayMode");

            public static void GetCurrentDisplayMode(int displayIndex, out Mode mode)
            {
                GetError(SDL_GetCurrentDisplayMode(displayIndex, out mode));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_getdisplaymode(int displayIndex, int modeIndex, out Mode mode);
            private static readonly d_sdl_getdisplaymode SDL_GetDisplayMode =
                FL.LoadFunction<d_sdl_getdisplaymode>(NativeLibrary, "SDL_GetDisplayMode");

            public static void GetDisplayMode(int displayIndex, int modeIndex, out Mode mode)
            {
                GetError(SDL_GetDisplayMode(displayIndex, modeIndex, out mode));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_getclosestdisplaymode(int displayIndex, Mode mode, out Mode closest);
            private static readonly d_sdl_getclosestdisplaymode SDL_GetClosestDisplayMode = 
                FL.LoadFunction<d_sdl_getclosestdisplaymode>(NativeLibrary, "SDL_GetClosestDisplayMode");

            public static void GetClosestDisplayMode(int displayIndex, Mode mode, out Mode closest)
            {
                GetError(SDL_GetClosestDisplayMode(displayIndex, mode, out closest));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_getdisplayname(int index);
            private static readonly d_sdl_getdisplayname SDL_GetDisplayName = 
                FL.LoadFunction<d_sdl_getdisplayname>(NativeLibrary, "SDL_GetDisplayName");

            public static string GetDisplayName(int index)
            {
                return InteropHelpers.Utf8ToString(GetError(SDL_GetDisplayName(index)));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_getnumdisplaymodes(int displayIndex);
            private static readonly d_sdl_getnumdisplaymodes SDL_GetNumDisplayModes =
                FL.LoadFunction<d_sdl_getnumdisplaymodes>(NativeLibrary, "SDL_GetNumDisplayModes");

            public static int GetNumDisplayModes(int displayIndex)
            {
                return GetError(SDL_GetNumDisplayModes(displayIndex));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_getnumvideodisplays();
            private static readonly d_sdl_getnumvideodisplays SDL_GetNumVideoDisplays = 
                FL.LoadFunction<d_sdl_getnumvideodisplays>(NativeLibrary, "SDL_GetNumVideoDisplays");

            public static int GetNumVideoDisplays()
            {
                return GetError(SDL_GetNumVideoDisplays());
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_getwindowdisplayindex(IntPtr window);
            private static readonly d_sdl_getwindowdisplayindex SDL_GetWindowDisplayIndex =
                FL.LoadFunction<d_sdl_getwindowdisplayindex>(NativeLibrary, "SDL_GetWindowDisplayIndex");

            public static int GetWindowDisplayIndex(IntPtr window)
            {
                return GetError(SDL_GetWindowDisplayIndex(window));
            }
        }
    }
}