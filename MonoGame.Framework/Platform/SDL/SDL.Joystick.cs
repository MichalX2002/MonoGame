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
        public static class Joystick
        {
            [Flags]
            public enum Hat : byte
            {
                Centered = 0,
                Up = 1 << 0,
                Right = 1 << 1,
                Down = 1 << 2,
                Left = 1 << 3
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct DeviceEvent
            {
                public EventType Type;
                public uint TimeStamp;
                public int Which;
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_joystickclose(IntPtr joystick);
            public static d_sdl_joystickclose Close = 
                FL.LoadFunction<d_sdl_joystickclose>(NativeLibrary, "SDL_JoystickClose");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_joystickfrominstanceid(int joyid);
            private static d_sdl_joystickfrominstanceid JoystickFromInstanceId =
                FL.LoadFunction<d_sdl_joystickfrominstanceid>(NativeLibrary, "SDL_JoystickFromInstanceID");

            public static IntPtr FromInstanceId(int joyid)
            {
                return GetError(JoystickFromInstanceId(joyid));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate short d_sdl_joystickgetaxis(IntPtr joystick, int axis);
            public static d_sdl_joystickgetaxis GetAxis = 
                FL.LoadFunction<d_sdl_joystickgetaxis>(NativeLibrary, "SDL_JoystickGetAxis");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate byte d_sdl_joystickgetbutton(IntPtr joystick, int button);
            public static d_sdl_joystickgetbutton GetButton = 
                FL.LoadFunction<d_sdl_joystickgetbutton>(NativeLibrary, "SDL_JoystickGetButton");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_joystickname(IntPtr joystick);
            private static d_sdl_joystickname JoystickName = 
                FL.LoadFunction<d_sdl_joystickname>(NativeLibrary, "SDL_JoystickName");

            public static string GetName(IntPtr joystick)
            {
                var namePtr = JoystickName(joystick);
                try
                {
                    return InteropHelpers.Utf8ToString(namePtr);
                }
                finally
                {
                    Free(namePtr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate Guid d_sdl_joystickgetguid(IntPtr joystick);
            public static d_sdl_joystickgetguid GetGUID = 
                FL.LoadFunction<d_sdl_joystickgetguid>(NativeLibrary, "SDL_JoystickGetGUID");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate Hat d_sdl_joystickgethat(IntPtr joystick, int hat);
            public static d_sdl_joystickgethat GetHat = 
                FL.LoadFunction<d_sdl_joystickgethat>(NativeLibrary, "SDL_JoystickGetHat");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_joystickinstanceid(IntPtr joystick);
            public static d_sdl_joystickinstanceid InstanceID = 
                FL.LoadFunction<d_sdl_joystickinstanceid>(NativeLibrary, "SDL_JoystickInstanceID");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_joystickopen(int deviceIndex);
            private static readonly d_sdl_joystickopen SDL_JoystickOpen = 
                FL.LoadFunction<d_sdl_joystickopen>(NativeLibrary, "SDL_JoystickOpen");

            public static IntPtr Open(int deviceIndex)
            {
                return GetError(SDL_JoystickOpen(deviceIndex));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_joysticknumaxes(IntPtr joystick);
            private static readonly d_sdl_joysticknumaxes SDL_JoystickNumAxes = 
                FL.LoadFunction<d_sdl_joysticknumaxes>(NativeLibrary, "SDL_JoystickNumAxes");

            public static int NumAxes(IntPtr joystick)
            {
                return GetError(SDL_JoystickNumAxes(joystick));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_joysticknumbuttons(IntPtr joystick);
            private static readonly d_sdl_joysticknumbuttons SDL_JoystickNumButtons = 
                FL.LoadFunction<d_sdl_joysticknumbuttons>(NativeLibrary, "SDL_JoystickNumButtons");

            public static int NumButtons(IntPtr joystick)
            {
                return GetError(SDL_JoystickNumButtons(joystick));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_joysticknumhats(IntPtr joystick);
            private static readonly d_sdl_joysticknumhats SDL_JoystickNumHats =
                FL.LoadFunction<d_sdl_joysticknumhats>(NativeLibrary, "SDL_JoystickNumHats");

            public static int NumHats(IntPtr joystick)
            {
                return GetError(SDL_JoystickNumHats(joystick));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_numjoysticks();
            private static readonly d_sdl_numjoysticks SDL_NumJoysticks =
                FL.LoadFunction<d_sdl_numjoysticks>(NativeLibrary, "SDL_NumJoysticks");

            public static int NumJoysticks()
            {
                return GetError(SDL_NumJoysticks());
            }
        }
    }
}