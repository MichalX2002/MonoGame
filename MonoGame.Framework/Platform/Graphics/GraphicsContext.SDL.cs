// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Graphics;

namespace MonoGame.OpenGL
{
    internal class GraphicsContext : IGraphicsContext, IDisposable
    {
        private IntPtr _context;
        private IntPtr _windowHandle;

        public bool IsDisposed { get; private set; }

        public bool IsCurrent => true;

        public int SwapInterval
        {
            get => SDL.GL.GetSwapInterval();
            set => SDL.GL.SetSwapInterval(value);
        }

        public GraphicsContext(IWindowHandle window)
        {
            if (IsDisposed)
                return;

            SetWindowHandle(window);
            _context = SDL.GL.CreateContext(_windowHandle);

            // GL entry points must be loaded after the GL context creation,
            // otherwise some Windows drivers will return only GL 1.3 compatible functions
            try
            {
                GL.LoadEntryPoints();
            }
            catch (EntryPointNotFoundException ex)
            {
                throw new PlatformNotSupportedException(
                    "MonoGame requires OpenGL 3.0 compatible drivers, " +
                    "or either ARB_framebuffer_object or EXT_framebuffer_object extensions. " +
                    "Try updating your graphics drivers.", ex);
            }
        }

        public void MakeCurrent(IWindowHandle window)
        {
            if (IsDisposed)
                return;

            SetWindowHandle(window);
            SDL.GL.MakeCurrent(_windowHandle, _context);
        }

        public void SwapBuffers()
        {
            if (IsDisposed)
                return;

            SDL.GL.SwapWindow(_windowHandle);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            GraphicsDevice.DisposeContext(_context);
            _context = IntPtr.Zero;
            IsDisposed = true;
        }

        private void SetWindowHandle(IWindowHandle info)
        {
            _windowHandle = info?.GetPlatformWindowHandle() ?? IntPtr.Zero;
        }
    }
}
