// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.OpenGL
{
    public static partial class GL
    {
        private static void LoadPlatformEntryPoints()
        {
            BoundAPI = RenderAPI.GL;
        }

        private static T? TryLoadFunction<T>(string function)
            where T : Delegate
        {
            var ret = SDL.GL.GetProcAddress(function);

            if (ret == IntPtr.Zero)
                return default;

            return Marshal.GetDelegateForFunctionPointer<T>(ret);
        }

        private static T LoadFunction<T>(string function)
            where T : Delegate
        {
            return TryLoadFunction<T>(function) ?? throw new EntryPointNotFoundException(function);
        }

        private static IGraphicsContext PlatformCreateContext(IWindowHandle window)
        {
            return new GraphicsContext(window);
        }
    }
}

