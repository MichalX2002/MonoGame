// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using FL = MonoGame.Framework.FuncLoader;

namespace MonoGame
{
    internal static partial class SDL
    {
        public static class Window
        {
            public const int PosUndefined = 0x1FFF0000;
            public const int PosCentered = 0x2FFF0000;

            public enum EventId : byte
            {
                None,
                Shown,
                Hidden,
                Exposed,
                Moved,
                Resized,
                SizeChanged,
                Minimized,
                Maximized,
                Restored,
                Enter,
                Leave,
                FocusGained,
                FocusLost,
                Close,
            }

            [Flags]
            public enum State
            {
                Fullscreen = 0x00000001,
                OpenGL = 0x00000002,
                Shown = 0x00000004,
                Hidden = 0x00000008,
                Borderless = 0x00000010,
                Resizable = 0x00000020,
                Minimized = 0x00000040,
                Maximized = 0x00000080,
                Grabbed = 0x00000100,
                InputFocus = 0x00000200,
                MouseFocus = 0x00000400,
                FullscreenDesktop = 0x00001001,
                Foreign = 0x00000800,
                AllowHighDPI = 0x00002000,
                MouseCapture = 0x00004000
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Event
            {
                public EventType Type;
                public uint TimeStamp;
                public uint WindowId;
                public EventId EventId;
                private readonly byte padding1;
                private readonly byte padding2;
                private readonly byte padding3;
                public int Data1;
                public int Data2;
            }

            public enum SysWMType
            {
                Unknown,
                Windows,
                X11,
                Directfb,
                Cocoa,
                UIKit,
                Wayland,
                Mir,
                WinRt,
                Android
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SysWMinfo
            {
                public Version version;
                public SysWMType subsystem;
                public WindowInfo data;
            }

            public unsafe struct WindowInfo
            {
                public const int WindowInfoSizeInBytes = 64;

                public fixed byte dummy[WindowInfoSizeInBytes];

                public WindowInfo_WINDOWS Windows => UnsafeUtils.As<WindowInfo, WindowInfo_WINDOWS>(this);

                // TODO:
                //public WindowInfo_X11 X11 => UnsafeUtils.As<WindowInfo, WindowInfo_X11>(this);
                //public WindowInfo_UIKIT UIKit => UnsafeUtils.As<WindowInfo, WindowInfo_UIKIT>(this);
            }

            public struct WindowInfo_WINDOWS
            {
                public IntPtr window;
                public IntPtr hdc;
                public IntPtr hinstance;
            }

            // TODO:
            //public struct WindowInfo_X11
            //{
            //}

            //public struct WindowInfo_UIKIT
            //{
            //}

            //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            //public delegate void d_sdl_maximizewindow(IntPtr window);
            //public static readonly d_sdl_maximizewindow MaximizeWindow = 
            //    FuncLoader.LoadFunction<d_sdl_maximizewindow>(NativeLibrary, "SDL_MaximizeWindow");
            
            //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            //public delegate void d_sdl_restorewindow(IntPtr window);
            //public static readonly d_sdl_restorewindow RestoreWindow = 
            //    FuncLoader.LoadFunction<d_sdl_restorewindow>(NativeLibrary, "SDL_RestoreWindow");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_createwindow(string title, int x, int y, int w, int h, State flags);
            private static readonly d_sdl_createwindow SDL_CreateWindow =
                FL.LoadFunction<d_sdl_createwindow>(NativeLibrary, "SDL_CreateWindow");

            public static IntPtr Create(string title, int x, int y, int w, int h, State flags)
            {
                return GetError(SDL_CreateWindow(title, x, y, w, h, flags));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_destroywindow(IntPtr window);
            public static d_sdl_destroywindow Destroy = 
                FL.LoadFunction<d_sdl_destroywindow>(NativeLibrary, "SDL_DestroyWindow");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate uint d_sdl_getwindowid(IntPtr window);
            public static d_sdl_getwindowid GetWindowId = 
                FL.LoadFunction<d_sdl_getwindowid>(NativeLibrary, "SDL_GetWindowID");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_getwindowdisplayindex(IntPtr window);
            private static readonly d_sdl_getwindowdisplayindex SDL_GetWindowDisplayIndex = 
                FL.LoadFunction<d_sdl_getwindowdisplayindex>(NativeLibrary, "SDL_GetWindowDisplayIndex");

            public static int GetDisplayIndex(IntPtr window)
            {
                return GetError(SDL_GetWindowDisplayIndex(window));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate State d_sdl_getwindowflags(IntPtr window);
            public static d_sdl_getwindowflags GetWindowFlags = 
                FL.LoadFunction<d_sdl_getwindowflags>(NativeLibrary, "SDL_GetWindowFlags");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_setwindowicon(IntPtr window, IntPtr icon);
            public static d_sdl_setwindowicon SetIcon = 
                FL.LoadFunction<d_sdl_setwindowicon>(NativeLibrary, "SDL_SetWindowIcon");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_getwindowposition(IntPtr window, out int x, out int y);
            public static d_sdl_getwindowposition GetPosition =
                FL.LoadFunction<d_sdl_getwindowposition>(NativeLibrary, "SDL_GetWindowPosition");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_getwindowsize(IntPtr window, out int w, out int h);
            public static d_sdl_getwindowsize GetSize =
                FL.LoadFunction<d_sdl_getwindowsize>(NativeLibrary, "SDL_GetWindowSize");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_setwindowbordered(IntPtr window, int bordered);
            public static d_sdl_setwindowbordered SetBordered = 
                FL.LoadFunction<d_sdl_setwindowbordered>(NativeLibrary, "SDL_SetWindowBordered");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_setwindowfullscreen(IntPtr window, State flags);
            private static readonly d_sdl_setwindowfullscreen SDL_SetWindowFullscreen =
                FL.LoadFunction<d_sdl_setwindowfullscreen>(NativeLibrary, "SDL_SetWindowFullscreen");

            public static void SetFullscreen(IntPtr window, State flags)
            {
                GetError(SDL_SetWindowFullscreen(window, flags));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_setwindowposition(IntPtr window, int x, int y);
            public static d_sdl_setwindowposition SetPosition =
                FL.LoadFunction<d_sdl_setwindowposition>(NativeLibrary, "SDL_SetWindowPosition");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_setwindowresizable(IntPtr window, bool resizable);
            public static d_sdl_setwindowresizable SetResizable =
                FL.LoadFunction<d_sdl_setwindowresizable>(NativeLibrary, "SDL_SetWindowResizable");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_setwindowsize(IntPtr window, int w, int h);
            public static d_sdl_setwindowsize SetSize = 
                FL.LoadFunction<d_sdl_setwindowsize>(NativeLibrary, "SDL_SetWindowSize");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void d_sdl_setwindowtitle(IntPtr window, ref byte value);
            private static readonly d_sdl_setwindowtitle SDL_SetWindowTitle = 
                FL.LoadFunction<d_sdl_setwindowtitle>(NativeLibrary, "SDL_SetWindowTitle");

            public static void SetTitle(IntPtr handle, string title)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(title);
                SDL_SetWindowTitle(handle, ref bytes[0]);
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_showwindow(IntPtr window);
            public static d_sdl_showwindow Show = FL.LoadFunction<d_sdl_showwindow>(NativeLibrary, "SDL_ShowWindow");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate bool d_sdl_getwindowwminfo(IntPtr window, ref SysWMinfo sysWMinfo);
            public static d_sdl_getwindowwminfo GetWindowWMInfo = 
                FL.LoadFunction<d_sdl_getwindowwminfo>(NativeLibrary, "SDL_GetWindowWMInfo");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_getwindowborderssize(IntPtr window, out int top, out int left, out int right, out int bottom);
            public static d_sdl_getwindowborderssize GetBorderSize = 
                FL.LoadFunction<d_sdl_getwindowborderssize>(NativeLibrary, "SDL_GetWindowBordersSize");
        }
    }
}