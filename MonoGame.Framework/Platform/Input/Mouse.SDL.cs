// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Input
{
    public partial class Mouse
    {
        private IntPtr _windowHandle;

        internal MouseState State;

        internal int ScrollX;
        internal int ScrollY;

        /// <inheritdoc/>
        protected override void WindowHandleChanged()
        {
            _windowHandle = Window.GetPlatformWindowHandle();
        }

        /// <inheritdoc/>
        protected override void Disposing()
        {
            _windowHandle = IntPtr.Zero;

            base.Disposing();
        }

        private MouseState PlatformGetState()
        {
            if (_windowHandle == IntPtr.Zero)
                return default;

            var winFlags = SDL.Window.GetWindowFlags(_windowHandle);
            var state = SDL.Mouse.GetGlobalState(out int x, out int y);

            if ((winFlags & SDL.Window.State.MouseFocus) != 0)
            {
                // Window has mouse focus, position will be set from the motion event
                State.LeftButton = (state & SDL.Mouse.Button.Left) != 0 ? ButtonState.Pressed : ButtonState.Released;
                State.MiddleButton = (state & SDL.Mouse.Button.Middle) != 0 ? ButtonState.Pressed : ButtonState.Released;
                State.RightButton = (state & SDL.Mouse.Button.Right) != 0 ? ButtonState.Pressed : ButtonState.Released;
                State.XButton1 = (state & SDL.Mouse.Button.X1Mask) != 0 ? ButtonState.Pressed : ButtonState.Released;
                State.XButton2 = (state & SDL.Mouse.Button.X2Mask) != 0 ? ButtonState.Pressed : ButtonState.Released;

                State.HorizontalScroll = ScrollX;
                State.VerticalScroll = ScrollY;
            }
            else
            {
                // Window does not have mouse focus, we need to manually get the position
                var clientBounds = Window.Bounds;
                State.X = x - clientBounds.X;
                State.Y = y - clientBounds.Y;
            }

            return State;
        }

        private void PlatformSetPosition(int x, int y)
        {
            if (_windowHandle == IntPtr.Zero)
                return;

            State.X = x;
            State.Y = y;
            SDL.Mouse.WarpInWindow(_windowHandle, x, y);
        }

        private void PlatformSetCursor(MouseCursor cursor)
        {
            if (_windowHandle == IntPtr.Zero)
                return;

            SDL.Mouse.SetCursor(cursor.Handle);
        }
    }
}
