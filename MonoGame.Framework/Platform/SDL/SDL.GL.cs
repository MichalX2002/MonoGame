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
        public static class GL
        {
            public enum Attribute
            {
                RedSize,
                GreenSize,
                BlueSize,
                AlphaSize,
                BufferSize,
                DoubleBuffer,
                DepthSize,
                StencilSize,
                AccumRedSize,
                AccumGreenSize,
                AccumBlueSize,
                AccumAlphaSize,
                Stereo,
                MultiSampleBuffers,
                MultiSampleSamples,
                AcceleratedVisual,
                RetainedBacking,
                ContextMajorVersion,
                ContextMinorVersion,
                ContextEgl,
                ContextFlags,
                ContextProfileMAsl,
                ShareWithCurrentContext,
                FramebufferSRGBCapable,
                ContextReleaseBehaviour,
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_gl_createcontext(IntPtr window);
            private static readonly d_sdl_gl_createcontext SDL_GL_CreateContext =
                FL.LoadFunction<d_sdl_gl_createcontext>(NativeLibrary, "SDL_GL_CreateContext");

            public static IntPtr CreateContext(IntPtr window)
            {
                return GetError(SDL_GL_CreateContext(window));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_gl_deletecontext(IntPtr context);
            public static d_sdl_gl_deletecontext DeleteContext = 
                FL.LoadFunction<d_sdl_gl_deletecontext>(NativeLibrary, "SDL_GL_DeleteContext");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr d_sdl_gl_getcurrentcontext();
            private static readonly d_sdl_gl_getcurrentcontext SDL_GL_GetCurrentContext =
                FL.LoadFunction<d_sdl_gl_getcurrentcontext>(NativeLibrary, "SDL_GL_GetCurrentContext");

            public static IntPtr GetCurrentContext()
            {
                return GetError(SDL_GL_GetCurrentContext());
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate IntPtr d_sdl_gl_getprocaddress(string proc);
            public static d_sdl_gl_getprocaddress GetProcAddress =
                FL.LoadFunction<d_sdl_gl_getprocaddress>(NativeLibrary, "SDL_GL_GetProcAddress");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_gl_getswapinterval();
            public static d_sdl_gl_getswapinterval GetSwapInterval =
                FL.LoadFunction<d_sdl_gl_getswapinterval>(NativeLibrary, "SDL_GL_GetSwapInterval");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_gl_makecurrent(IntPtr window, IntPtr context);
            public static d_sdl_gl_makecurrent MakeCurrent =
                FL.LoadFunction<d_sdl_gl_makecurrent>(NativeLibrary, "SDL_GL_MakeCurrent");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int d_sdl_gl_setattribute(Attribute attr, int value);
            private static readonly d_sdl_gl_setattribute SDL_GL_SetAttribute =
                FL.LoadFunction<d_sdl_gl_setattribute>(NativeLibrary, "SDL_GL_SetAttribute");

            public static int SetAttribute(Attribute attr, int value)
            {
                return GetError(SDL_GL_SetAttribute(attr, value));
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int d_sdl_gl_setswapinterval(int interval);
            public static d_sdl_gl_setswapinterval SetSwapInterval =
                FL.LoadFunction<d_sdl_gl_setswapinterval>(NativeLibrary, "SDL_GL_SetSwapInterval");

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void d_sdl_gl_swapwindow(IntPtr window);
            public static d_sdl_gl_swapwindow SwapWindow = 
                FL.LoadFunction<d_sdl_gl_swapwindow>(NativeLibrary, "SDL_GL_SwapWindow");
        }
    }
}