// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework
{
    internal class SdlGameWindow : GameWindow, IDisposable
    {
        public override bool AllowUserResizing
        {
            get => !IsBorderless && _resizable;
            set
            {
                if (SDL.Major >= 2 && SDL.Minor >= 0 && SDL.Patch > 4)
                {
                    SDL.Window.SetResizable(_handle, value);
                }
                else
                {
                    string version = string.Join(".", SDL.Major, SDL.Minor, SDL.Patch);
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
                SDL.Window.GetPosition(Handle, out int x, out int y);
                return new Rectangle(x, y, _width, _height);
            }
        }

        public override Point Position
        {
            get
            {
                if (!IsFullScreen)
                {
                    SDL.Window.GetPosition(Handle, out int x, out int y);
                    return new Point(x, y);
                }
                return Point.Zero;
            }
            set
            {
                SDL.Window.SetPosition(Handle, value.X, value.Y);
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
                SDL.Window.SetBordered(_handle, value ? 0 : 1);
                _borderless = value;
            }
        }

        public override bool HasClipboardText => SDL.HasClipboardText();
        public override string ClipboardText { get => SDL.GetClipboardText(); set => SDL.SetClipboardText(value); }

        public static GameWindow Instance;
        public bool IsFullScreen;

        internal readonly Game _game;
        private IntPtr _handle;
        private IntPtr _icon;
        private string _defaultTitle;
        private bool _disposed;
        private bool _resizable, _borderless, _willBeFullScreen, _mouseVisible, _hardwareSwitch;
        private string _screenDeviceName;
        private int _width, _height;
        private bool _wasMoved, _supressMoved;

        public SdlGameWindow(Game game)
        {
            Instance = this;

            _game = game;
            _screenDeviceName = "";
            _width = GraphicsDeviceManager.DefaultBackBufferWidth;
            _height = GraphicsDeviceManager.DefaultBackBufferHeight;
            _defaultTitle = AssemblyHelper.GetDefaultWindowTitle();

            byte[] iconBytes = ReadEmbeddedIconBytes();
            if (iconBytes != null)
            {
                var dataSrc = SDL.RwFromMem(iconBytes, iconBytes.Length);
                _icon = SDL.LoadBMP_RW(dataSrc, 1);
            }

            SDL.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "0");
            SDL.SetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS", "1");

            _handle = SDL.Window.Create(
                _defaultTitle,
                x: 0, y: 0,
                GraphicsDeviceManager.DefaultBackBufferWidth,
                GraphicsDeviceManager.DefaultBackBufferHeight,
                SDL.Window.State.Hidden);
        }

        public IntPtr GetOSWindowHandle()
        {
            if (_handle != IntPtr.Zero)
            {
                SDL.Window.SysWMinfo wmInfo = default;
                SDL.GetVersion(out wmInfo.version);
                if (SDL.Window.GetWindowWMInfo(_handle, ref wmInfo))
                {
                    if (wmInfo.subsystem == SDL.Window.SysWMType.Windows)
                    {
                        return wmInfo.data.Windows.window;
                    }
                }
            }
            return IntPtr.Zero;
        }

        internal void CreateWindow()
        {
            var initflags =
                SDL.Window.State.Hidden |
                SDL.Window.State.OpenGL |
                SDL.Window.State.InputFocus |
                SDL.Window.State.MouseFocus;

            if (_handle != IntPtr.Zero)
                SDL.Window.Destroy(_handle);

            int winx = SDL.Window.PosCentered;
            int winy = SDL.Window.PosCentered;

            // if we are on Linux, start on the current screen
            if (PlatformInfo.OS == PlatformInfo.OperatingSystem.Linux)
            {
                winx |= GetMouseDisplay();
                winy |= GetMouseDisplay();
            }

            _handle = SDL.Window.Create(
                _defaultTitle, winx, winy, _width, _height, initflags);

            if (_icon != IntPtr.Zero)
                SDL.Window.SetIcon(_handle, _icon);

            SDL.Window.SetBordered(_handle, _borderless ? 0 : 1);
            SDL.Window.SetResizable(_handle, _resizable);

            SetCursorVisible(_mouseVisible);
        }

        private static byte[] ReadEmbeddedIconBytes()
        {
            // when running NUnit tests entry assembly can be null
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                var entryDeclaringType = entryAssembly.EntryPoint.DeclaringType;
                using (var stream =
                        entryAssembly.GetManifestResourceStream(entryDeclaringType.Namespace + ".Icon.bmp") ??
                        entryAssembly.GetManifestResourceStream("Icon.bmp") ??
                        Assembly.GetExecutingAssembly().GetManifestResourceStream("MonoGame.bmp"))
                {
                    if (stream != null)
                    {
                        using (var br = new BinaryReader(stream))
                        {
                            try
                            {
                                int length = (int)stream.Length;
                                return br.ReadBytes(length);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Failed to load icon from embedded resources: {0}", ex);
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static int GetMouseDisplay()
        {
            var rect = new SDL.Rectangle();
            SDL.Mouse.GetGlobalState(out int x, out int y);

            var displayCount = SDL.Display.GetNumVideoDisplays();
            for (int i = 0; i < displayCount; i++)
            {
                SDL.Display.GetBounds(i, out rect);

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
            SDL.Mouse.ShowCursor(visible ? 1 : 0);
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            _willBeFullScreen = willBeFullScreen;
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            _screenDeviceName = screenDeviceName;

            var prevBounds = ClientBounds;
            var displayIndex = SDL.Window.GetDisplayIndex(Handle);

            SDL.Display.GetBounds(displayIndex, out SDL.Rectangle displayRect);

            if (_willBeFullScreen != IsFullScreen ||
                _hardwareSwitch != _game.GraphicsDeviceManager.HardwareModeSwitch)
            {
                var fullscreenFlag = _game.GraphicsDeviceManager.HardwareModeSwitch
                    ? SDL.Window.State.Fullscreen
                    : SDL.Window.State.FullscreenDesktop;

                SDL.Window.SetFullscreen(Handle, _willBeFullScreen ? fullscreenFlag : 0);
                _hardwareSwitch = _game.GraphicsDeviceManager.HardwareModeSwitch;
            }

            // If going to exclusive full-screen mode, force the window to minimize on focus loss (Windows only)
            if (PlatformInfo.OS == PlatformInfo.OperatingSystem.Windows)
                SDL.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", _willBeFullScreen && _hardwareSwitch ? "1" : "0");
            
            if (!_willBeFullScreen || _game.GraphicsDeviceManager.HardwareModeSwitch)
            {
                SDL.Window.SetSize(Handle, clientWidth, clientHeight);
                _width = clientWidth;
                _height = clientHeight;
            }
            else
            {
                _width = displayRect.Width;
                _height = displayRect.Height;
            }

            SDL.Window.GetBorderSize(_handle, out int miny, out int minx, out _, out _);

            var centerX = Math.Max(prevBounds.X + ((prevBounds.Width - clientWidth) / 2), minx);
            var centerY = Math.Max(prevBounds.Y + ((prevBounds.Height - clientHeight) / 2), miny);

            if (IsFullScreen && !_willBeFullScreen)
            {
                // We need to get the display information again in case
                // the resolution of it was changed.
                SDL.Display.GetBounds(displayIndex, out displayRect);

                // This centering only occurs when exiting fullscreen
                // so it should center the window on the current display.
                centerX = displayRect.X + displayRect.Width / 2 - clientWidth / 2;
                centerY = displayRect.Y + displayRect.Height / 2 - clientHeight / 2;
            }

            // If this window is resizable, there is a bug in SDL 2.0.4 where
            // after the window gets resized, window position information
            // becomes wrong (for me it always returned 10 8). Solution is
            // to not try and set the window position because it will be wrong.
            if ((SDL.Patch > 4 || !AllowUserResizing) && !_wasMoved)
                SDL.Window.SetPosition(Handle, centerX, centerY);

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

            SDL.Window.GetSize(Handle, out _width, out _height);
            OnClientSizeChanged();
        }

        protected internal override void SetSupportedOrientations(DisplayOrientation orientations)
        {
            // Nothing to do here
        }

        protected override void SetTitle(string title)
        {
            SDL.Window.SetTitle(_handle, title);
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

            SDL.Window.Destroy(_handle);
            _handle = IntPtr.Zero;

            if (_icon != IntPtr.Zero)
                SDL.FreeSurface(_icon);

            _disposed = true;
        }

        ~SdlGameWindow()
        {
            Dispose(false);
        }
    }
}
