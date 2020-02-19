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
        private IntPtr _winHandle;

        public int SwapInterval
        {
            get => SDL.GL.GetSwapInterval();
            set => SDL.GL.SetSwapInterval(value);
        }

        public bool IsDisposed { get; private set; }

        public bool IsCurrent => true;

        public GraphicsContext(IWindowInfo info)
        {
            if (IsDisposed)
                return;
            
            SetWindowHandle(info);
            _context = SDL.GL.CreateContext(_winHandle);

            // GL entry points must be loaded after the GL context creation, otherwise some Windows drivers will return only GL 1.3 compatible functions
            try
            {
                GL.LoadEntryPoints();
            }
            catch (EntryPointNotFoundException)
            {
                throw new PlatformNotSupportedException(
                    "MonoGame requires OpenGL 3.0 compatible drivers, or either ARB_framebuffer_object or EXT_framebuffer_object extensions. " +
                    "Try updating your graphics drivers.");
            }
        }

        public void MakeCurrent(IWindowInfo info)
        {
            if (IsDisposed)
                return;
            
            SetWindowHandle(info);
            SDL.GL.MakeCurrent(_winHandle, _context);
        }

        public void SwapBuffers()
        {
            if (IsDisposed)
                return;
            
            SDL.GL.SwapWindow(_winHandle);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            GraphicsDevice.DisposeContext(_context);
            _context = IntPtr.Zero;
            IsDisposed = true;
        }

        private void SetWindowHandle(IWindowInfo info)
        {
            if (info == null)
                _winHandle = IntPtr.Zero;
            else
                _winHandle = info.Handle;
        }
    }
}
