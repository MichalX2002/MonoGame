// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Runtime.InteropServices;
using FL = MonoGame.Framework.FuncLoader;

namespace MonoGame
{
    internal static partial class SDL
    {
        public static unsafe class Touch
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct FingerEvent
            {
                public EventType Type;
                public uint Timestamp;
                public long TouchId;
                public long FingerId;
                public float X;
                public float Y;
                public float DX;
                public float DY;
                public float Pressure;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MultiGestureEvent
            {
                public EventType Type;
                public uint Timestamp;
                public long TouchId;
                public float DTheta;
                public float DDist;
                public float X;
                public float Y;
                public ushort NumFingers;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Finger
            {
                public long Id;
                public float X;
                public float Y;
                public float Pressure;
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_SDL_GetNumTouchDevices();
            public static readonly d_SDL_GetNumTouchDevices SDL_GetNumTouchDevices =
                FL.LoadFunction<d_SDL_GetNumTouchDevices>(NativeLibrary, "SDL_GetNumTouchDevices");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate long d_SDL_GetTouchDevice(int index);
            public static readonly d_SDL_GetTouchDevice SDL_GetTouchDevice =
                FL.LoadFunction<d_SDL_GetTouchDevice>(NativeLibrary, "SDL_GetTouchDevice");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate long d_SDL_GetNumTouchFingers(long touchId);
            public static readonly d_SDL_GetNumTouchFingers SDL_GetNumTouchFingers =
                FL.LoadFunction<d_SDL_GetNumTouchFingers>(NativeLibrary, "SDL_GetNumTouchFingers");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate Finger* d_SDL_GetTouchFinger(long touchId, int index);
            public static readonly d_SDL_GetTouchFinger SDL_GetTouchFinger =
                FL.LoadFunction<d_SDL_GetTouchFinger>(NativeLibrary, "SDL_GetTouchFinger");
        }
    }
}