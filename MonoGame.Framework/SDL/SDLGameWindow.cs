// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Utilities;
using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Xna.Framework
{
    internal class SdlGameWindow : GameWindow, IDisposable
    {
        public override bool AllowUserResizing
        {
            get => !IsBorderless && _resizable;
            set
            {
                if (Sdl.Major >= 2 && Sdl.Minor >= 0 && Sdl.Patch > 4)
                    Sdl.Window.SetResizable(_handle, value);
                else
                {
                    string version = string.Join(".", Sdl.Major, Sdl.Minor, Sdl.Patch);
                    throw new Exception(
                        $"SDL {version} does not support changing the resizable parameter of the window after it's already been created.");
                }
                _resizable = value;
            }
        }

        public override Rectangle ClientBounds
        {
            get
            {
                Sdl.Window.GetPosition(Handle, out int x, out int y);
                return new Rectangle(x, y, _width, _height);
            }
        }

        public override Point Position
        {
            get
            {
                int x = 0, y = 0;

                if (!IsFullScreen)
                    Sdl.Window.GetPosition(Handle, out x, out y);

                return new Point(x, y);
            }
            set
            {
                Sdl.Window.SetPosition(Handle, value.X, value.Y);
                _wasMoved = true;
            }
        }

        public override DisplayOrientation CurrentOrientation => DisplayOrientation.Default;
        public override IntPtr Handle => _handle;
        public override string ScreenDeviceName => _screenDeviceName;

        public override bool IsBorderless
        {
            get => _borderless;
            set
            {
                Sdl.Window.SetBordered(_handle, value ? 0 : 1);
                _borderless = value;
            }
        }

        public override bool HasClipboardText => Sdl.HasClipboardText();
        public override string ClipboardText { get => Sdl.GetClipboardText(); set => Sdl.SetClipboardText(value); }

        public static GameWindow Instance;
        public uint? ID;
        public bool IsFullScreen;

        internal readonly Game _game;
        private IntPtr _handle;
        private IntPtr _icon;
        private bool _disposed;
        private bool _resizable, _borderless, _willBeFullScreen, _mouseVisible, _hardwareSwitch;
        private string _screenDeviceName;
        private int _width, _height;
        private bool _wasMoved, _supressMoved;

        public SdlGameWindow(Game game)
        {
            _game = game;
            _screenDeviceName = "";

            Instance = this;

            _width = GraphicsDeviceManager.DefaultBackBufferWidth;
            _height = GraphicsDeviceManager.DefaultBackBufferHeight;

            Sdl.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "0");
            Sdl.SetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS", "1");

            // when running NUnit tests entry assembly can be null
            if (Assembly.GetEntryAssembly() != null)
            {
                using (var stream =
                        Assembly.GetEntryAssembly().GetManifestResourceStream(Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace + ".Icon.bmp") ??
                        Assembly.GetEntryAssembly().GetManifestResourceStream("Icon.bmp") ??
                        Assembly.GetExecutingAssembly().GetManifestResourceStream("MonoGame.bmp"))
                {
                    if (stream != null)
                        using (var br = new BinaryReader(stream))
                        {
                            try
                            {
                                int length = (int)stream.Length;
                                var src = Sdl.RwFromMem(br.ReadBytes(length), length);
                                _icon = Sdl.LoadBMP_RW(src, 1);
                            }
                            catch
                            {
                            }
                        }
                }
            }

            _handle = Sdl.Window.Create("", 0, 0,
                GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight, Sdl.Window.State.Hidden);
        }

        internal void InitTaskbarList(IntPtr windowHandle)
        {
            _taskbarList = new Utilities.TaskbarList(windowHandle);
            _taskbarList.SetProgressState(TaskbarState);
            _taskbarList.SetProgressValue(TaskbarProgress);
        }

        internal void CreateWindow()
        {
            var initflags =
                Sdl.Window.State.OpenGL |
                Sdl.Window.State.Hidden |
                Sdl.Window.State.InputFocus |
                Sdl.Window.State.MouseFocus;

            if (_handle != IntPtr.Zero)
                Sdl.Window.Destroy(_handle);

            var winx = Sdl.Window.PosCentered;
            var winy = Sdl.Window.PosCentered;

            // if we are on Linux, start on the current screen
            if (CurrentPlatform.OS == OS.Linux)
            {
                winx |= GetMouseDisplay();
                winy |= GetMouseDisplay();
            }

            _handle = Sdl.Window.Create(
                AssemblyHelper.GetDefaultWindowTitle(), winx, winy, _width, _height, initflags);

            ID = Sdl.Window.GetWindowId(_handle);
            if (_icon != IntPtr.Zero)
                Sdl.Window.SetIcon(_handle, _icon);

            Sdl.Window.SetBordered(_handle, _borderless ? 0 : 1);
            Sdl.Window.SetResizable(_handle, _resizable);

            SetCursorVisible(_mouseVisible);
        }

        ~SdlGameWindow()
        {
            Dispose(false);
        }

        private static int GetMouseDisplay()
        {
            var rect = new Sdl.Rectangle();
            Sdl.Mouse.GetGlobalState(out int x, out int y);

            var displayCount = Sdl.Display.GetNumVideoDisplays();
            for (var i = 0; i < displayCount; i++)
            {
                Sdl.Display.GetBounds(i, out rect);

                if (x >= rect.X && x < rect.X + rect.Width &&
                    y >= rect.Y && y < rect.Y + rect.Height)
                {
                    return i;
                }
            }

            return 0;
        }

        public void SetCursorVisible(bool visible)
        {
            _mouseVisible = visible;
            Sdl.Mouse.ShowCursor(visible ? 1 : 0);
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            _willBeFullScreen = willBeFullScreen;
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            _screenDeviceName = screenDeviceName;

            var prevBounds = ClientBounds;
            var displayIndex = Sdl.Window.GetDisplayIndex(Handle);

            Sdl.Display.GetBounds(displayIndex, out Sdl.Rectangle displayRect);

            if (_willBeFullScreen != IsFullScreen || _hardwareSwitch != _game.InternalGraphicsDeviceManager.HardwareModeSwitch)
            {
                var fullscreenFlag = _game.InternalGraphicsDeviceManager.HardwareModeSwitch ? Sdl.Window.State.Fullscreen : Sdl.Window.State.FullscreenDesktop;
                Sdl.Window.SetFullscreen(Handle, (_willBeFullScreen) ? fullscreenFlag : 0);
                _hardwareSwitch = _game.InternalGraphicsDeviceManager.HardwareModeSwitch;
            }
            // If going to exclusive full-screen mode, force the window to minimize on focus loss (Windows only)
            if (CurrentPlatform.OS == OS.Windows)
            {
                Sdl.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", _willBeFullScreen && _hardwareSwitch ? "1" : "0");
            }

            if (!_willBeFullScreen || _game.InternalGraphicsDeviceManager.HardwareModeSwitch)
            {
                Sdl.Window.SetSize(Handle, clientWidth, clientHeight);
                _width = clientWidth;
                _height = clientHeight;
            }
            else
            {
                _width = displayRect.Width;
                _height = displayRect.Height;
            }

            Sdl.Window.GetBorderSize(_handle, out int miny, out int minx, out _, out _);

            var centerX = Math.Max(prevBounds.X + ((prevBounds.Width - clientWidth) / 2), minx);
            var centerY = Math.Max(prevBounds.Y + ((prevBounds.Height - clientHeight) / 2), miny);

            if (IsFullScreen && !_willBeFullScreen)
            {
                // We need to get the display information again in case
                // the resolution of it was changed.
                Sdl.Display.GetBounds(displayIndex, out displayRect);

                // This centering only occurs when exiting fullscreen
                // so it should center the window on the current display.
                centerX = displayRect.X + displayRect.Width / 2 - clientWidth / 2;
                centerY = displayRect.Y + displayRect.Height / 2 - clientHeight / 2;
            }

            // If this window is resizable, there is a bug in SDL 2.0.4 where
            // after the window gets resized, window position information
            // becomes wrong (for me it always returned 10 8). Solution is
            // to not try and set the window position because it will be wrong.
            if ((Sdl.Patch > 4 || !AllowUserResizing) && !_wasMoved)
                Sdl.Window.SetPosition(Handle, centerX, centerY);

            if (IsFullScreen != _willBeFullScreen)
                OnClientSizeChanged();

            IsFullScreen = _willBeFullScreen;

            _supressMoved = true;
        }

        internal void Moved()
        {
            if (_supressMoved)
            {
                _supressMoved = false;
                return;
            }

            _wasMoved = true;
        }

        public void ClientResize(int width, int height)
        {
            // SDL reports many resize events even if the Size didn't change.
            // Only call the code below if it actually changed.
            if (_game.GraphicsDevice.PresentationParameters.BackBufferWidth == width &&
                _game.GraphicsDevice.PresentationParameters.BackBufferHeight == height)
            {
                return;
            }
            _game.GraphicsDevice.PresentationParameters.BackBufferWidth = width;
            _game.GraphicsDevice.PresentationParameters.BackBufferHeight = height;
            _game.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);

            Sdl.Window.GetSize(Handle, out _width, out _height);

            OnClientSizeChanged();
        }

        public void CallTextInput(int character, Keys key)
        {
            OnTextInput(this, character, key);
        }

        protected internal override void SetSupportedOrientations(DisplayOrientation orientations)
        {
            // Nothing to do here
        }

        protected override void SetTitle(string title)
        {
            Sdl.Window.SetTitle(_handle, title);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            Sdl.Window.Destroy(_handle);
            _handle = IntPtr.Zero;

            if (_icon != IntPtr.Zero)
                Sdl.FreeSurface(_icon);

            _disposed = true;
        }
    }
}
