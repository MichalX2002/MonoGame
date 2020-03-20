// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using FL = MonoGame.Framework.FuncLoader;

namespace MonoGame
{
    internal static partial class SDL
    {
        public static class Haptic
        {
            // For some reason, different game controllers have different maximum value supported
            // Also, the more the value is close to their limit, the more they tend to randomly ignore it
            // Hence, we're setting an abitrary safe value as a maximum
            public const uint Infinity = 1000000U;

            public enum EffectId : ushort
            {
                LeftRight = 1 << 2,
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct LeftRight
            {
                public EffectId Type;
                public uint Length;
                public ushort LargeMagnitude;
                public ushort SmallMagnitude;
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct Effect
            {
                [FieldOffset(0)] public EffectId type;
                [FieldOffset(0)] public LeftRight leftright;
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_hapticclose(IntPtr haptic);
            public static d_sdl_hapticclose Close =
                FL.LoadFunction<d_sdl_hapticclose>(NativeLibrary, "SDL_HapticClose");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_hapticeffectsupported(IntPtr haptic, ref Effect effect);
            public static d_sdl_hapticeffectsupported EffectSupported = 
                FL.LoadFunction<d_sdl_hapticeffectsupported>(NativeLibrary, "SDL_HapticEffectSupported");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_joystickishaptic(IntPtr joystick);
            public static d_sdl_joystickishaptic IsHaptic =
                FL.LoadFunction<d_sdl_joystickishaptic>(NativeLibrary, "SDL_JoystickIsHaptic");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_hapticneweffect(IntPtr haptic, ref Effect effect);
            private static readonly d_sdl_hapticneweffect HapticNewEffect = 
                FL.LoadFunction<d_sdl_hapticneweffect>(NativeLibrary, "SDL_HapticNewEffect");

            public static void NewEffect(IntPtr haptic, ref Effect effect)
            {
                GetError(HapticNewEffect(haptic, ref effect));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate IntPtr d_sdl_hapticopen(int device_index);
            public static d_sdl_hapticopen Open =
                FL.LoadFunction<d_sdl_hapticopen>(NativeLibrary, "SDL_HapticOpen");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_hapticopenfromjoystick(IntPtr joystick);
            private static readonly d_sdl_hapticopenfromjoystick HapticOpenFromJoystick = 
                FL.LoadFunction<d_sdl_hapticopenfromjoystick>(NativeLibrary, "SDL_HapticOpenFromJoystick");

            public static IntPtr OpenFromJoystick(IntPtr joystick)
            {
                return GetError(HapticOpenFromJoystick(joystick));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_hapticrumbleinit(IntPtr haptic);
            private static readonly d_sdl_hapticrumbleinit HapticRumbleInit = 
                FL.LoadFunction<d_sdl_hapticrumbleinit>(NativeLibrary, "SDL_HapticRumbleInit");

            public static void RumbleInit(IntPtr haptic)
            {
                GetError(HapticRumbleInit(haptic));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_hapticrumbleplay(IntPtr haptic, float strength, uint length);
            private static readonly d_sdl_hapticrumbleplay HapticRumblePlay =
                FL.LoadFunction<d_sdl_hapticrumbleplay>(NativeLibrary, "SDL_HapticRumblePlay");

            public static void RumblePlay(IntPtr haptic, float strength, uint length)
            {
                GetError(HapticRumblePlay(haptic, strength, length));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_hapticrumblesupported(IntPtr haptic);
            private static readonly d_sdl_hapticrumblesupported HapticRumbleSupported = 
                FL.LoadFunction<d_sdl_hapticrumblesupported>(NativeLibrary, "SDL_HapticRumbleSupported");

            public static int RumbleSupported(IntPtr haptic)
            {
                return GetError(HapticRumbleSupported(haptic));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_hapticruneffect(IntPtr haptic, int effect, uint iterations);
            private static readonly d_sdl_hapticruneffect HapticRunEffect =
                FL.LoadFunction<d_sdl_hapticruneffect>(NativeLibrary, "SDL_HapticRunEffect");

            public static void RunEffect(IntPtr haptic, int effect, uint iterations)
            {
                GetError(HapticRunEffect(haptic, effect, iterations));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_hapticstopall(IntPtr haptic);
            private static readonly d_sdl_hapticstopall HapticStopAll =
                FL.LoadFunction<d_sdl_hapticstopall>(NativeLibrary, "SDL_HapticStopAll");

            public static void StopAll(IntPtr haptic)
            {
                GetError(HapticStopAll(haptic));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_hapticupdateeffect(IntPtr haptic, int effect, ref Effect data);
            private static readonly d_sdl_hapticupdateeffect HapticUpdateEffect =
                FL.LoadFunction<d_sdl_hapticupdateeffect>(NativeLibrary, "SDL_HapticUpdateEffect");

            public static void UpdateEffect(IntPtr haptic, int effect, ref Effect data)
            {
                GetError(HapticUpdateEffect(haptic, effect, ref data));
            }
        }
    }
}