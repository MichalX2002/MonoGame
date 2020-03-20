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
        public static class GameController
        {
            public enum Axis
            {
                Invalid = -1,
                LeftX,
                LeftY,
                RightX,
                RightY,
                TriggerLeft,
                TriggerRight,
                Max,
            }

            public enum Button
            {
                Invalid = -1,
                A,
                B,
                X,
                Y,
                Back,
                Guide,
                Start,
                LeftStick,
                RightStick,
                LeftShoulder,
                RightShoulder,
                DpadUp,
                DpadDown,
                DpadLeft,
                DpadRight,
                Max,
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct DeviceEvent
            {
                public EventType Type;
                public uint TimeStamp;
                public int Which;
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_free(IntPtr ptr);
            public static d_sdl_free SDL_Free = 
                FL.LoadFunction<d_sdl_free>(NativeLibrary, "SDL_free");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_gamecontrolleraddmapping(string mappingString);
            public static d_sdl_gamecontrolleraddmapping AddMapping = 
                FL.LoadFunction<d_sdl_gamecontrolleraddmapping>(NativeLibrary, "SDL_GameControllerAddMapping");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_gamecontrolleraddmappingsfromrw(IntPtr rw, int freew);
            public static d_sdl_gamecontrolleraddmappingsfromrw AddMappingFromRw =
                FL.LoadFunction<d_sdl_gamecontrolleraddmappingsfromrw>(NativeLibrary, "SDL_GameControllerAddMappingsFromRW");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_gamecontrollerclose(IntPtr gamecontroller);
            public static d_sdl_gamecontrollerclose Close =
                FL.LoadFunction<d_sdl_gamecontrollerclose>(NativeLibrary, "SDL_GameControllerClose");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_joystickfrominstanceid(int joyid);
            private static readonly d_sdl_joystickfrominstanceid GameControllerFromInstanceId =
                FL.LoadFunction<d_sdl_joystickfrominstanceid>(NativeLibrary, "SDL_JoystickFromInstanceID");

            public static IntPtr FromInstanceId(int joyid)
            {
                return GetError(GameControllerFromInstanceId(joyid));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate short d_sdl_gamecontrollergetaxis(IntPtr gamecontroller, Axis axis);
            public static d_sdl_gamecontrollergetaxis GetAxis =
                FL.LoadFunction<d_sdl_gamecontrollergetaxis>(NativeLibrary, "SDL_GameControllerGetAxis");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate byte d_sdl_gamecontrollergetbutton(IntPtr gamecontroller, Button button);
            public static d_sdl_gamecontrollergetbutton GetButton = 
                FL.LoadFunction<d_sdl_gamecontrollergetbutton>(NativeLibrary, "SDL_GameControllerGetButton");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_gamecontrollergetjoystick(IntPtr gamecontroller);
            private static readonly d_sdl_gamecontrollergetjoystick GameControllerGetJoystick = 
                FL.LoadFunction<d_sdl_gamecontrollergetjoystick>(NativeLibrary, "SDL_GameControllerGetJoystick");

            public static IntPtr GetJoystick(IntPtr gamecontroller)
            {
                return GetError(GameControllerGetJoystick(gamecontroller));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate byte d_sdl_isgamecontroller(int joystickIndex);
            public static d_sdl_isgamecontroller IsGameController =
                FL.LoadFunction<d_sdl_isgamecontroller>(NativeLibrary, "SDL_IsGameController");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_gamecontrollermapping(IntPtr gamecontroller);
            private static readonly d_sdl_gamecontrollermapping GameControllerMapping = 
                FL.LoadFunction<d_sdl_gamecontrollermapping>(NativeLibrary, "SDL_GameControllerMapping");

            public static string GetMapping(IntPtr gamecontroller)
            {
                IntPtr mappingStr = GameControllerMapping(gamecontroller);
                if (mappingStr == IntPtr.Zero)
                    return string.Empty;

                try
                {
                    return InteropHelpers.Utf8ToString(mappingStr);
                }
                finally
                {
                    //The mapping string returned by SDL is owned by us and thus must be freed
                    SDL_Free(mappingStr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_gamecontrolleropen(int joystickIndex);
            private static readonly d_sdl_gamecontrolleropen GameControllerOpen =
                FL.LoadFunction<d_sdl_gamecontrolleropen>(NativeLibrary, "SDL_GameControllerOpen");

            public static IntPtr Open(int joystickIndex)
            {
                return GetError(GameControllerOpen(joystickIndex));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_gamecontrollername(IntPtr gamecontroller);
            private static readonly d_sdl_gamecontrollername GameControllerName = 
                FL.LoadFunction<d_sdl_gamecontrollername>(NativeLibrary, "SDL_GameControllerName");

            public static string GetName(IntPtr gamecontroller)
            {
                return InteropHelpers.Utf8ToString(GameControllerName(gamecontroller));
            }
        }
    }
}