﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Framework.Input.Touch;
using MonoGame.Framework.Windows;

using ButtonState = MonoGame.Framework.Input.ButtonState;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;
using Keys = MonoGame.Framework.Input.Keys;

namespace MonoGame.Framework
{
    internal class WinFormsGameWindow : GameWindow, IDisposable
    {
        internal WinFormsGameForm Form;

        private static ReaderWriterLockSlim _allWindowsReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private static List<WinFormsGameWindow> _allWindows = new List<WinFormsGameWindow>();

        private WinFormsGamePlatform _platform;

        private FormWindowState _lastFormState;

        private bool _isResizable;
        private bool _isBorderless;
        private bool _isMouseHidden;
        private bool _isMouseInBounds;

        private DrawingPoint _locationBeforeFullScreen;
        // flag to indicate that we're switching to/from full screen and should ignore resize events
        private bool _switchingFullScreen;

        // true if window position was moved either through code or by dragging/resizing the form
        private bool _wasMoved;

        private bool _isResizeTickEnabled;
        private System.Timers.Timer? _resizeTickTimer;

        internal Game Game { get; private set; }

        public override IntPtr WindowHandle => Form.Handle;
        public override string ScreenDeviceName => string.Empty;
        public override DisplayOrientation CurrentOrientation => DisplayOrientation.Default;

        public override Rectangle Bounds
        {
            get
            {
                var position = Form.PointToScreen(DrawingPoint.Empty);
                var size = Form.ClientSize;
                return new Rectangle(position.X, position.Y, size.Width, size.Height);
            }
        }

        public override bool AllowUserResizing
        {
            get => _isResizable;
            set
            {
                if (_isResizable != value)
                {
                    _isResizable = value;
                    Form.MaximizeBox = _isResizable;
                }
                else
                    return;
                if (_isBorderless)
                    return;
                Form.FormBorderStyle = _isResizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
            }
        }

        public override Point Position
        {
            get => new Point(Form.Location.X, Form.Location.Y);
            set
            {
                _wasMoved = true;
                Form.Location = new DrawingPoint(value.X, value.Y);
                RefreshAdapter();
            }
        }

        protected internal override void SetSupportedOrientations(DisplayOrientation orientations)
        {
        }

        public override bool IsBorderless
        {
            get => _isBorderless;
            set
            {
                if (_isBorderless != value)
                    _isBorderless = value;
                else
                    return;
                if (_isBorderless)
                    Form.FormBorderStyle = FormBorderStyle.None;
                else
                    Form.FormBorderStyle = _isResizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
            }
        }

        public bool IsFullScreen { get; private set; }
        public bool HardwareModeSwitch { get; private set; }

        public override bool HasClipboardText => Clipboard.ContainsText();
        public override string ClipboardText { get => Clipboard.GetText(); set => Clipboard.SetText(value); }

        internal WinFormsGameWindow(WinFormsGamePlatform platform) : base()
        {
            _platform = platform;
            Game = platform.Game;

            Form = new WinFormsGameForm(this);
            ChangeSize(new DrawingSize(
                GraphicsDeviceManager.DefaultBackBufferWidth,
                GraphicsDeviceManager.DefaultBackBufferHeight));

            SetIcon();
            Title = AssemblyHelper.GetDefaultWindowTitle();

            Form.MaximizeBox = false;
            Form.FormBorderStyle = FormBorderStyle.FixedSingle;
            Form.StartPosition = FormStartPosition.Manual;

            // Capture mouse events.
            Form.MouseWheel += OnMouseScroll;
            Form.MouseHorizontalWheel += OnMouseHorizontalScroll;
            Form.MouseEnter += OnMouseEnter;
            Form.MouseLeave += OnMouseLeave;

            Form.KeyPress += OnKeyPress;

            _resizeTickTimer = new System.Timers.Timer(1) { SynchronizingObject = Form, AutoReset = false };
            _resizeTickTimer.Elapsed += OnResizeTick;

            Form.Activated += OnActivated;
            Form.Deactivate += OnDeactivate;
            Form.Resize += OnResize;
            Form.ResizeBegin += OnResizeBegin;
            Form.ResizeEnd += OnResizeEnd;

            Form.HandleCreated += Form_HandleCreated;
            Form.HandleDestroyed += Form_HandleDestroyed;

            RegisterToAllWindows();
        }

        private void Form_HandleCreated(object? sender, EventArgs e)
        {
            OnWindowHandleChanged();
        }

        private void Form_HandleDestroyed(object? sender, EventArgs e)
        {
            OnWindowHandleChanged();
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINTSTRUCT
        {
            public int X;
            public int Y;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string exeFileName, int iconIndex);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out POINTSTRUCT pt);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern int MapWindowPoints(HandleRef hWndFrom, HandleRef hWndTo, out POINTSTRUCT pt, int cPoints);

        private void SetIcon()
        {
            // When running unit tests this can return null.
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                var handle = ExtractIcon(IntPtr.Zero, assembly.Location, 0);
                if (handle != IntPtr.Zero)
                    Form.Icon = Icon.FromHandle(handle);
            }
        }

        public override IntPtr GetSubsystemWindowHandle()
        {
            return Form.Handle;
        }

        private void RegisterToAllWindows()
        {
            _allWindowsReaderWriterLockSlim.EnterWriteLock();
            try
            {
                _allWindows.Add(this);
            }
            finally
            {
                _allWindowsReaderWriterLockSlim.ExitWriteLock();
            }
        }

        private void UnregisterFromAllWindows()
        {
            _allWindowsReaderWriterLockSlim.EnterWriteLock();
            try
            {
                _allWindows.Remove(this);
            }
            finally
            {
                _allWindowsReaderWriterLockSlim.ExitWriteLock();
            }
        }

        private void OnActivated(object? sender, EventArgs eventArgs)
        {
            _platform.IsActive = true;
            Keyboard.SetActive(true);
        }

        private void OnDeactivate(object? sender, EventArgs eventArgs)
        {
            // If in exclusive mode full-screen, force it out of exclusive mode and minimize the window
            if (IsFullScreen && _platform.Game.GraphicsDevice.PresentationParameters.HardwareModeSwitch)
            {
                // This is true when the user presses the Windows key while game window has focus
                if (Form.WindowState == FormWindowState.Minimized)
                    MinimizeFullScreen();
            }
            _platform.IsActive = false;
            Keyboard.SetActive(false);
        }

        private void OnMouseScroll(object? sender, MouseEventArgs mouseEventArgs)
        {
            Mouse.State.VerticalScroll += mouseEventArgs.Delta;
        }

        private void OnMouseHorizontalScroll(object sender, HorizontalMouseWheelEvent mouseEventArgs)
        {
            Mouse.State.HorizontalScroll += mouseEventArgs.Delta;
        }

        private void UpdateMouseState()
        {
            // If we call the form client functions before the form has
            // been made visible it will cause the wrong window size to
            // be applied at startup.
            if (!Form.Visible)
                return;

            GetCursorPos(out _);
            MapWindowPoints(new HandleRef(null, IntPtr.Zero), new HandleRef(Form, Form.Handle), out POINTSTRUCT pos, 1);

            var clientPos = new DrawingPoint(pos.X, pos.Y);
            var withinClient = Form.ClientRectangle.Contains(clientPos);
            var buttons = Control.MouseButtons;

            var previousLeftButton = Mouse.State.LeftButton;

            Mouse.State.X = clientPos.X;
            Mouse.State.Y = clientPos.Y;
            Mouse.State.LeftButton = (buttons & MouseButtons.Left) == MouseButtons.Left ? ButtonState.Pressed : ButtonState.Released;
            Mouse.State.MiddleButton = (buttons & MouseButtons.Middle) == MouseButtons.Middle ? ButtonState.Pressed : ButtonState.Released;
            Mouse.State.RightButton = (buttons & MouseButtons.Right) == MouseButtons.Right ? ButtonState.Pressed : ButtonState.Released;
            Mouse.State.XButton1 = (buttons & MouseButtons.XButton1) == MouseButtons.XButton1 ? ButtonState.Pressed : ButtonState.Released;
            Mouse.State.XButton2 = (buttons & MouseButtons.XButton2) == MouseButtons.XButton2 ? ButtonState.Pressed : ButtonState.Released;

            // Don't process touch state if we're not active 
            // and the mouse is within the client area.
            if (!_platform.IsActive || !withinClient)
            {
                if (Mouse.State.LeftButton == ButtonState.Pressed)
                {
                    // Release mouse TouchLocation
                    var touchX = MathHelper.Clamp(Mouse.State.X, 0, Form.ClientRectangle.Width - 1);
                    var touchY = MathHelper.Clamp(Mouse.State.Y, 0, Form.ClientRectangle.Height - 1);
                    TouchPanel.AddEvent(0, TouchLocationState.Released, new Vector2(touchX, touchY), true);
                }
                return;
            }

            TouchLocationState? touchState = null;
            if (Mouse.State.LeftButton == ButtonState.Pressed)
                if (previousLeftButton == ButtonState.Released)
                    touchState = TouchLocationState.Pressed;
                else
                    touchState = TouchLocationState.Moved;
            else if (previousLeftButton == ButtonState.Pressed)
                touchState = TouchLocationState.Released;

            if (touchState.HasValue)
                TouchPanel.AddEvent(0, touchState.Value, new Vector2(Mouse.State.X, Mouse.State.Y), true);
        }

        private void OnMouseEnter(object? sender, EventArgs e)
        {
            _isMouseInBounds = true;
            if (!_platform.IsMouseVisible && !_isMouseHidden)
            {
                _isMouseHidden = true;
                Cursor.Hide();
            }
        }

        private void OnMouseLeave(object? sender, EventArgs e)
        {
            _isMouseInBounds = false;
            if (_isMouseHidden)
            {
                _isMouseHidden = false;
                Cursor.Show();
            }
        }

        [DllImport("user32.dll")]
        private static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

        private void OnKeyPress(object? sender, KeyPressEventArgs e)
        {
            var key = (Keys)(VkKeyScanEx(e.KeyChar, InputLanguage.CurrentInputLanguage.Handle) & 0xff);
            OnTextInput(new TextInputEventArgs(new Rune(e.KeyChar), key));
        }

        internal void Initialize(int width, int height)
        {
            ChangeSize(new DrawingSize(width, height));
        }

        internal void Initialize(PresentationParameters pp)
        {
            ChangeSize(new DrawingSize(pp.BackBufferWidth, pp.BackBufferHeight));

            if (pp.IsFullScreen)
            {
                EnterFullScreen(pp);

                if (!pp.HardwareModeSwitch)
                    _platform.Game.GraphicsDevice.OnPresentationChanged();
            }
        }

        private void OnResize(object? sender, EventArgs eventArgs)
        {
            if (_switchingFullScreen || Form.IsResizing)
                return;

            // this event can be triggered when moving the window through Windows hotkeys
            // in that case we should no longer center the window after resize
            if (_lastFormState == Form.WindowState)
                _wasMoved = true;

            if (Game.Window == this && Form.WindowState != FormWindowState.Minimized)
            {
                // we may need to restore full screen when coming back from a minimized window
                if (_lastFormState == FormWindowState.Minimized)
                    _platform.Game.GraphicsDevice.SetHardwareFullscreen();
                UpdateBackBufferSize();
            }

            _lastFormState = Form.WindowState;
            OnSizeChanged();
        }

        private void OnResizeBegin(object? sender, EventArgs e)
        {
            _isResizeTickEnabled = true;
            _resizeTickTimer!.Enabled = true;
        }

        private void OnResizeTick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_isResizeTickEnabled)
                return;

            UpdateWindows();
            Game.Tick();
            _resizeTickTimer!.Enabled = true;
        }

        private void OnResizeEnd(object? sender, EventArgs eventArgs)
        {
            _isResizeTickEnabled = false;
            _resizeTickTimer!.Enabled = false;

            _wasMoved = true;
            if (Game.Window == this)
            {
                UpdateBackBufferSize();
                RefreshAdapter();
            }

            OnSizeChanged();
        }

        private void RefreshAdapter()
        {
            // the display that the window is on might have changed, so we need to
            // check and possibly update the Adapter of the GraphicsDevice
            Game.GraphicsDevice?.RefreshAdapter();
        }

        private void UpdateBackBufferSize()
        {
            var manager = Game.GraphicsDeviceManager;
            if (manager.GraphicsDevice == null)
                return;

            var newSize = Form.ClientSize;
            if (newSize.Width == manager.PreferredBackBufferWidth
                && newSize.Height == manager.PreferredBackBufferHeight)
                return;

            // Set the default new back buffer size
            manager.PreferredBackBufferWidth = newSize.Width;
            manager.PreferredBackBufferHeight = newSize.Height;
            manager.ApplyChanges();
        }

        protected override void SetTitle(ReadOnlySpan<char> title)
        {
            Form.Text = title.ToString();
        }

        internal void RunLoop()
        {
            Application.Idle += TickOnIdle;
            Application.Run(Form);
            Application.Idle -= TickOnIdle;

            // We need to remove the WM_QUIT message in the message pump 
            // as it will keep us from restarting on this same thread.
            //
            // This is critical for some NUnit runners which typically 
            // will run all the tests on the same process/thread.
            var msg = new NativeMessage();
            do
            {
                if (msg.msg == WM_QUIT)
                    break;

                Thread.Sleep(100);
            }
            while (PeekMessage(out msg, IntPtr.Zero, 0, 1 << 5, 1));
        }

        // Run game loop when the app becomes Idle.
        private void TickOnIdle(object? sender, EventArgs e)
        {
            do
            {
                UpdateWindows();
                Game.Tick();
            }
            while (!PeekMessage(out _, IntPtr.Zero, 0, 0, 0) && Form != null && Form.IsDisposed == false);
        }

        internal void UpdateWindows()
        {
            _allWindowsReaderWriterLockSlim.EnterReadLock();

            try
            {
                // Update the mouse state for each window.
                foreach (var window in _allWindows)
                    if (window.Game == Game)
                        window.UpdateMouseState();
            }
            finally
            {
                _allWindowsReaderWriterLockSlim.ExitReadLock();
            }
        }

        private const uint WM_QUIT = 0x12;

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }

        internal void ChangeSize(DrawingSize bounds)
        {
            var prevIsResizing = Form.IsResizing;
            // make sure we don't see the events from this as a user resize
            Form.IsResizing = true;

            if (Form.ClientSize != bounds)
                Form.ClientSize = bounds;

            // if the window wasn't moved manually and it's resized, it should be centered
            if (!_wasMoved)
                Form.CenterOnPrimaryMonitor();

            Form.IsResizing = prevIsResizing;
        }

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(
            out NativeMessage msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
        }

        public void MouseVisibleToggled()
        {
            if (_platform.IsMouseVisible)
            {
                if (_isMouseHidden)
                {
                    Cursor.Show();
                    _isMouseHidden = false;
                }
            }
            else if (!_isMouseHidden && _isMouseInBounds)
            {
                Cursor.Hide();
                _isMouseHidden = true;
            }
        }

        internal void OnPresentationChanged(PresentationParameters pp)
        {
            var raiseSizeChanged = false;
            if (pp.IsFullScreen && pp.HardwareModeSwitch && IsFullScreen && HardwareModeSwitch)
            {
                if (_platform.IsActive)
                {
                    // stay in hardware full screen, need to call ResizeTargets so the displaymode can be switched
                    _platform.Game.GraphicsDevice.ResizeTargets();
                }
                else
                {
                    // This needs to be called in case the user presses the Windows key while the focus is on the second monitor,
                    //	which (sometimes) causes the window to exit fullscreen mode, but still keeps it visible
                    MinimizeFullScreen();
                }
            }
            else if (pp.IsFullScreen && (!IsFullScreen || pp.HardwareModeSwitch != HardwareModeSwitch))
            {
                EnterFullScreen(pp);
                raiseSizeChanged = true;
            }
            else if (!pp.IsFullScreen && IsFullScreen)
            {
                ExitFullScreen();
                raiseSizeChanged = true;
            }

            ChangeSize(new DrawingSize(pp.BackBufferWidth, pp.BackBufferHeight));

            if (raiseSizeChanged)
                OnSizeChanged();
        }

        private void EnterFullScreen(PresentationParameters pp)
        {
            _switchingFullScreen = true;

            // store the location of the window so we can restore it later
            if (!IsFullScreen)
                _locationBeforeFullScreen = Form.Location;

            _platform.Game.GraphicsDevice.SetHardwareFullscreen();

            if (!pp.HardwareModeSwitch)
            {
                // FIXME: setting the WindowState to Maximized when the form is not shown will not update the ClientBounds
                // this causes the back buffer to be the wrong size when initializing in soft full screen
                // we show the form to bypass the issue
                Form.Show();
                IsBorderless = true;
                Form.WindowState = FormWindowState.Maximized;
                _lastFormState = FormWindowState.Maximized;
            }

            IsFullScreen = true;
            HardwareModeSwitch = pp.HardwareModeSwitch;

            _switchingFullScreen = false;
        }


        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        private void ExitFullScreen()
        {
            _switchingFullScreen = true;

            _platform.Game.GraphicsDevice.ClearHardwareFullscreen();

            IsBorderless = false;
            Form.WindowState = FormWindowState.Normal;
            _lastFormState = FormWindowState.Normal;
            Form.Location = _locationBeforeFullScreen;
            IsFullScreen = false;

            // Windows does not always correctly redraw the desktop when exiting soft full screen, so force a redraw
            if (!HardwareModeSwitch)
                RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 1);

            _switchingFullScreen = false;
        }

        private void MinimizeFullScreen()
        {
            _switchingFullScreen = true;

            _platform.Game.GraphicsDevice.ClearHardwareFullscreen();

            IsBorderless = false;
            Form.WindowState = FormWindowState.Minimized;
            _lastFormState = FormWindowState.Minimized;
            Form.Location = _locationBeforeFullScreen;
            IsFullScreen = false;

            // Windows does not always correctly redraw the desktop when exiting soft full screen, so force a redraw
            if (!HardwareModeSwitch)
                RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 1);

            _switchingFullScreen = false;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Form != null)
                {
                    UnregisterFromAllWindows();
                    Form.Dispose();
                    Form = null!;
                }

                _resizeTickTimer?.Dispose();
                _resizeTickTimer = null;
            }

            _platform = null!;
            Game = null!;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WinFormsGameWindow()
        {
            Dispose(false);
        }
    }
}

