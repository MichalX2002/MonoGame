// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using FL = MonoGame.Framework.FuncLoader;

namespace MonoGame
{
    [Guid("DA23ADEA-3FBC-41B8-B748-F378E9C7BB24")]
    internal static partial class SDL
    {
        public static IntPtr NativeLibrary = GetNativeLibrary();

        private static IntPtr GetNativeLibrary()
        {
            if (PlatformInfo.OS == PlatformInfo.OperatingSystem.Windows)
                return FL.LoadLibraryExt("SDL2.dll");
            else if (PlatformInfo.OS == PlatformInfo.OperatingSystem.Linux)
                return FL.LoadLibraryExt("libSDL2-2.0.so.0");
            else if (PlatformInfo.OS == PlatformInfo.OperatingSystem.MacOSX)
                return FL.LoadLibraryExt("libSDL2-2.0.0.dylib");
            else
                return FL.LoadLibraryExt("sdl2");
        }

        [Flags]
        public enum InitFlags
        {
            Video = 0x00000020,
            Joystick = 0x00000200,
            Haptic = 0x00001000,
            GameController = 0x00002000,
        }

        public enum EventState : int
        {
            Query = -1,
            Ignore = 0,
            Enable = 1,
        }

        public enum EventType : uint
        {
            First = 0,

            Quit = 0x100,

            WindowEvent = 0x200,
            SysWM = 0x201,

            KeyDown = 0x300,
            KeyUp = 0x301,
            TextEditing = 0x302,
            TextInput = 0x303,

            MouseMotion = 0x400,
            MouseButtonDown = 0x401,
            MouseButtonup = 0x402,
            MouseWheel = 0x403,

            JoyAxisMotion = 0x600,
            JoyBallMotion = 0x601,
            JoyHatMotion = 0x602,
            JoyButtonDown = 0x603,
            JoyButtonUp = 0x604,
            JoyDeviceAdded = 0x605,
            JoyDeviceRemoved = 0x606,

            ControllerAxisMotion = 0x650,
            ControllerButtonDown = 0x651,
            ControllerButtonUp = 0x652,
            ControllerDeviceAdded = 0x653,
            ControllerDeviceRemoved = 0x654,
            ControllerDeviceRemapped = 0x654,

            FingerDown = 0x700,
            FingerUp = 0x701,
            FingerMotion = 0x702,

            DollarGesture = 0x800,
            DollarRecord = 0x801,
            MultiGesture = 0x802,

            ClipboardUpdate = 0x900,

            DropFile = 0x1000,
            DropText = 0x1001,
            DropBegin = 0x1002,
            DropCompleted = 0x1003,

            AudioDeviceAdded = 0x1100,
            AudioDeviceRemoved = 0x1101,

            RenderTargetsReset = 0x2000,
            RenderDeviceReset = 0x2001,

            UserEvent = 0x8000,

            Last = 0xFFFF
        }

        public enum EventAction
        {
            AddEvent = 0x0,
            PeekEvent = 0x1,
            GetEvent = 0x2,
        }

        [StructLayout(LayoutKind.Explicit, Size = 56)]
        public struct Event
        {
            [FieldOffset(0)] public EventType Type;
            [FieldOffset(0)] public Window.Event Window;
            [FieldOffset(0)] public Keyboard.Event Key;
            [FieldOffset(0)] public Mouse.MotionEvent Motion;
            [FieldOffset(0)] public Keyboard.TextEditingEvent Edit;
            [FieldOffset(0)] public Keyboard.TextInputEvent Text;
            [FieldOffset(0)] public Mouse.WheelEvent Wheel;
            [FieldOffset(0)] public Joystick.DeviceEvent JoystickDevice;
            [FieldOffset(0)] public GameController.DeviceEvent ControllerDevice;
            [FieldOffset(0)] public Drop.DropEvent Drop;
        }

        public struct Rectangle
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        public struct Version
        {
            public byte Major;
            public byte Minor;
            public byte Patch;

            public override string ToString()
            {
                return string.Join(".", Major, Minor, Patch);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_init(int flags);
        private static d_sdl_init SDL_Init =
            FL.LoadFunction<d_sdl_init>(NativeLibrary, "SDL_Init");

        public static void Init(int flags)
        {
            GetError(SDL_Init(flags));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_hasclipboardtext();
        private static readonly d_sdl_hasclipboardtext SDL_HasClipboardText =
            FL.LoadFunction<d_sdl_hasclipboardtext>(NativeLibrary, "SDL_HasClipboardText");

        public static bool HasClipboardText()
        {
            return SDL_HasClipboardText() > 0 ? true : false;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_getclipboardtext();
        private static readonly d_sdl_getclipboardtext SDL_GetClipboardText =
            FL.LoadFunction<d_sdl_getclipboardtext>(NativeLibrary, "SDL_GetClipboardText");

        public static string GetClipboardText()
        {
            if (!HasClipboardText())
                return string.Empty;

            IntPtr nativeStr = SDL_GetClipboardText();
            try
            {
                return InteropHelpers.Utf8ToString(GetError(nativeStr));
            }
            finally
            {
                Free(nativeStr);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_sdl_free(IntPtr pointer);
        public static readonly d_sdl_free Free =
            FL.LoadFunction<d_sdl_free>(NativeLibrary, "SDL_free");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_setclipboardtext(string value);
        public static readonly d_sdl_setclipboardtext SetClipboardText =
            FL.LoadFunction<d_sdl_setclipboardtext>(NativeLibrary, "SDL_SetClipboardText");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_disablescreensaver();
        public static d_sdl_disablescreensaver DisableScreenSaver =
            FL.LoadFunction<d_sdl_disablescreensaver>(NativeLibrary, "SDL_DisableScreenSaver");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_getversion(out Version version);
        public static d_sdl_getversion GetVersion =
            FL.LoadFunction<d_sdl_getversion>(NativeLibrary, "SDL_GetVersion");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_pollevent([Out] out Event _event);
        public static d_sdl_pollevent PollEvent =
            FL.LoadFunction<d_sdl_pollevent>(NativeLibrary, "SDL_PollEvent");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_pumpevents();
        public static d_sdl_pumpevents PumpEvents =
            FL.LoadFunction<d_sdl_pumpevents>(NativeLibrary, "SDL_PumpEvents");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate EventState d_sdl_eventstate(EventType type, EventState state);
        public static d_sdl_eventstate SetEventState =
            FL.LoadFunction<d_sdl_eventstate>(NativeLibrary, "SDL_EventState");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_creatergbsurfacefrom(
            IntPtr pixels, int width, int height, int depth, int pitch, uint rMask, uint gMask, uint bMask, uint aMask);
        private static readonly d_sdl_creatergbsurfacefrom SDL_CreateRGBSurfaceFrom =
            FL.LoadFunction<d_sdl_creatergbsurfacefrom>(NativeLibrary, "SDL_CreateRGBSurfaceFrom");

        public static IntPtr CreateRGBSurfaceFrom(
            IntPtr pixelBuffer, int width, int height, int depth, int pitch, uint rMask, uint gMask, uint bMask, uint aMask)
        {
            return SDL_CreateRGBSurfaceFrom(pixelBuffer, width, height, depth, pitch, rMask, gMask, bMask, aMask);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_freesurface(IntPtr surface);
        public static d_sdl_freesurface FreeSurface =
            FL.LoadFunction<d_sdl_freesurface>(NativeLibrary, "SDL_FreeSurface");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_geterror();
        private static readonly d_sdl_geterror SDL_GetError =
            FL.LoadFunction<d_sdl_geterror>(NativeLibrary, "SDL_GetError");

        public static string GetError()
        {
            return InteropHelpers.Utf8ToString(SDL_GetError());
        }

        public static int GetError(int value)
        {
            if (value < 0)
                Debug.WriteLine(GetError());
            return value;
        }

        public static IntPtr GetError(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                Debug.WriteLine(GetError());
            return pointer;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_clearerror();
        public static d_sdl_clearerror ClearError =
            FL.LoadFunction<d_sdl_clearerror>(NativeLibrary, "SDL_ClearError");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_sdl_gethint(string name);
        public static d_sdl_gethint SDL_GetHint =
            FL.LoadFunction<d_sdl_gethint>(NativeLibrary, "SDL_GetHint");

        public static string GetHint(string name)
        {
            return InteropHelpers.Utf8ToString(SDL_GetHint(name));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_loadbmp_rw(IntPtr src, int freesrc);
        private static readonly d_sdl_loadbmp_rw SDL_LoadBMP_RW =
            FL.LoadFunction<d_sdl_loadbmp_rw>(NativeLibrary, "SDL_LoadBMP_RW");

        public static IntPtr LoadBMP_RW(IntPtr src, int freesrc)
        {
            return GetError(SDL_LoadBMP_RW(src, freesrc));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_quit();
        public static d_sdl_quit Quit =
            FL.LoadFunction<d_sdl_quit>(NativeLibrary, "SDL_Quit");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_rwfrommem(byte[] mem, int size);
        private static readonly d_sdl_rwfrommem SDL_RWFromMem =
            FL.LoadFunction<d_sdl_rwfrommem>(NativeLibrary, "SDL_RWFromMem");

        public static IntPtr RwFromMem(byte[] mem, int size)
        {
            return GetError(SDL_RWFromMem(mem, size));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_sethint(string name, string value);
        public static d_sdl_sethint SetHint =
            FL.LoadFunction<d_sdl_sethint>(NativeLibrary, "SDL_SetHint");
    }
}