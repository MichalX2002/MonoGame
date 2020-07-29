// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MonoGame.Framework.Input
{
    public partial class Mouse
    {
        internal new WinFormsGameWindow Window => (WinFormsGameWindow)base.Window;

        internal MouseState State;

        private Control? _window;
        private Cursor? _cursor;

        /// <inheritdoc/>
        protected override void WindowHandleChanged()
        {
            _window = Control.FromHandle(Window.GetPlatformWindowHandle());

            UpdateMousePosition(); // TODO: check if this is smart
            UpdateCursor();
        }

        private MouseState PlatformGetState()
        {
            return State;
        }

        private void PlatformSetPosition(int x, int y)
        {
            State.X = x;
            State.Y = y;
            UpdateMousePosition();
        }

        private void PlatformSetCursor(MouseCursor cursor)
        {
            _cursor = cursor.Cursor;
            UpdateCursor();
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        private void UpdateMousePosition()
        {
            if (_window != null)
            {
                var pt = _window.PointToScreen(new System.Drawing.Point(State.X, State.Y));
                SetCursorPos(pt.X, pt.Y);
            }
        }

        private void UpdateCursor()
        {
            if (_window != null)
            {
                if (_cursor != null)
                    _window.Cursor = _cursor;
            }
        }
    }
}
