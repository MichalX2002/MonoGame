// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Input
{
    public partial class MouseCursor
    {
        private MouseCursor(SDL.Mouse.SystemCursor cursor)
        {
            Handle = SDL.Mouse.CreateSystemCursor(cursor);
        }

        private static void PlatformInitalize()
        {
            Arrow = new MouseCursor(SDL.Mouse.SystemCursor.Arrow);
            IBeam = new MouseCursor(SDL.Mouse.SystemCursor.IBeam);
            Wait = new MouseCursor(SDL.Mouse.SystemCursor.Wait);
            Crosshair = new MouseCursor(SDL.Mouse.SystemCursor.Crosshair);
            WaitArrow = new MouseCursor(SDL.Mouse.SystemCursor.WaitArrow);
            SizeNWSE = new MouseCursor(SDL.Mouse.SystemCursor.SizeNWSE);
            SizeNESW = new MouseCursor(SDL.Mouse.SystemCursor.SizeNESW);
            SizeWE = new MouseCursor(SDL.Mouse.SystemCursor.SizeWE);
            SizeNS = new MouseCursor(SDL.Mouse.SystemCursor.SizeNS);
            SizeAll = new MouseCursor(SDL.Mouse.SystemCursor.SizeAll);
            No = new MouseCursor(SDL.Mouse.SystemCursor.No);
            Hand = new MouseCursor(SDL.Mouse.SystemCursor.Hand);
        }

        private static unsafe MouseCursor PlatformFromPixels(
            ReadOnlySpan<Color> data, int width, int height, Point origin)
        {
            var surface = IntPtr.Zero;
            var handle = IntPtr.Zero;
            try
            {
                fixed (Color* ptr = &MemoryMarshal.GetReference(data))
                {
                    surface = SDL.CreateRGBSurfaceFrom(
                        (IntPtr)ptr, width, height, 32, width * sizeof(Color),
                        0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);
                }

                if (surface == IntPtr.Zero)
                    throw new InvalidOperationException(
                        "Failed to create surface for mouse cursor.", new Exception(SDL.GetError()));

                handle = SDL.Mouse.CreateColorCursor(surface, origin.X, origin.Y);
                if (handle == IntPtr.Zero)
                    throw new InvalidOperationException(
                        "Failed to create mouse cursor from surface.", new Exception(SDL.GetError()));
            }
            finally
            {
                if (surface != IntPtr.Zero)
                    SDL.FreeSurface(surface);
            }
            return new MouseCursor(handle);
        }

        private void PlatformDispose()
        {
            if (Handle == IntPtr.Zero)
                return;

            SDL.Mouse.FreeCursor(Handle);
            Handle = IntPtr.Zero;
        }
    }
}
