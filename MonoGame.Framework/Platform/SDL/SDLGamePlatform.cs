// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
    internal class SDLGamePlatform : GamePlatform
    {
        private const int MouseWheelDelta = 120;

        private readonly Game _game;

        private int _isExiting;
        private SDLGameWindow _window;

        private List<string>? _fileDropList;

        public override GameRunBehavior DefaultRunBehavior => GameRunBehavior.Synchronous;

        public override GameWindow Window => _window;

        public SDLGamePlatform(Game game) : base(game)
        {
            _game = game;

            SDL.GetVersion(out SDL.Version version);

            if (version.Major < 2 || version.Minor < 0 || version.Patch < 5)
                Debug.WriteLine("Please use SDL 2.0.5 or higher.");

            // Needed so VS can debug the project on Windows
            if (version.Major >= 2 && version.Minor >= 0 && version.Patch >= 5 &&
                Debugger.IsAttached &&
                PlatformInfo.OS == PlatformInfo.OperatingSystem.Windows)
                SDL.SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");

            SDL.Init(
                SDL.InitFlags.Video |
                SDL.InitFlags.Joystick |
                SDL.InitFlags.GameController |
                SDL.InitFlags.Haptic);

            SDL.DisableScreenSaver();

            GamePad.InitDatabase();
            _window = new SDLGameWindow(_game);
        }

        public override void BeforeInitialize()
        {
            PollSDLEvents();

            base.BeforeInitialize();
        }

        protected override void OnIsMouseVisibleChanged()
        {
            _window.SetCursorVisible(_game.IsMouseVisible);
        }

        internal override void OnPresentationChanged(PresentationParameters pp)
        {
            var displayIndex = SDL.Window.GetDisplayIndex(_window.WindowHandle);
            var displayName = SDL.Display.GetDisplayName(displayIndex);
            BeginScreenDeviceChange(pp.IsFullScreen);
            EndScreenDeviceChange(displayName, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public override void RunLoop()
        {
            SDL.Window.Show(_window.WindowHandle);
            Window.OnWindowHandleChanged();

            while (true)
            {
                PollSDLEvents();
                Keyboard._modifiers = SDL.Keyboard.GetModState();

                Game.Tick();

                Threading.Run();
                GraphicsDevice.DisposeContexts();

                if (_isExiting > 0)
                    break;
            }
        }

        private void PollSDLEvents()
        {
            while (SDL.PollEvent(out SDL.Event ev) == 1)
            {
                switch (ev.Type)
                {
                    case SDL.EventType.Quit:
                        Interlocked.Increment(ref _isExiting);
                        break;

                    #region Joystick

                    case SDL.EventType.JoyDeviceAdded:
                        Joystick.AddDevice(ev.JoystickDevice.Which);
                        break;

                    case SDL.EventType.JoyDeviceRemoved:
                        Joystick.RemoveDevice(ev.JoystickDevice.Which);
                        break;
                            
                    #endregion

                    #region GameController

                    case SDL.EventType.ControllerDeviceRemoved:
                        GamePad.RemoveDevice(ev.ControllerDevice.Which);
                        break;

                    case SDL.EventType.ControllerButtonUp:
                    case SDL.EventType.ControllerButtonDown:
                    case SDL.EventType.ControllerAxisMotion:
                        GamePad.UpdatePacketInfo(ev.ControllerDevice.Which, ev.ControllerDevice.TimeStamp);
                        break;

                    #endregion

                    #region Mouse

                    case SDL.EventType.MouseWheel:
                        _window.Mouse.ScrollY += ev.MouseWheel.Y * MouseWheelDelta;
                        _window.Mouse.ScrollX += ev.MouseWheel.X * MouseWheelDelta;
                        break;

                    case SDL.EventType.MouseMotion:
                        _window.Mouse.State.X = ev.MouseMotion.X;
                        _window.Mouse.State.Y = ev.MouseMotion.Y;
                        break;

                    #endregion

                    #region Keyboard

                    case SDL.EventType.KeyDown:
                    {
                        bool hasMapping = KeyboardUtil.ToXna(ev.KeyboardKey.Keysym.Sym, out var key);
                        if (hasMapping)
                            if (!Keyboard._keysDown.Contains(key))
                                Keyboard._keysDown.Add(key);

                        // TODO: validate rune?
                        Rune.TryCreate(ev.KeyboardKey.Keysym.Sym, out var rune);

                        var inputEv = new TextInputEventArgs(rune, hasMapping ? key : (Keys?)null);
                        _window.OnKeyDown(inputEv);

                        if (Rune.IsControl(rune))
                            _window.OnTextInput(inputEv);
                        break;
                    }

                    case SDL.EventType.KeyUp:
                    {
                        bool hasMapping = KeyboardUtil.ToXna(ev.KeyboardKey.Keysym.Sym, out var key);
                        if (hasMapping)
                            Keyboard._keysDown.Remove(key);

                        // TODO: validate rune?
                        Rune.TryCreate(ev.KeyboardKey.Keysym.Sym, out var rune);

                        _window.OnKeyUp(new TextInputEventArgs(rune, hasMapping ? key : (Keys?)null));
                        break;
                    }

                    case SDL.EventType.TextInput:
                        unsafe
                        {
                            var utf8 = new Span<byte>(ev.TextInput.Text, SDL.Keyboard.TextInputEvent.TextSize);
                            int length = utf8.IndexOf((byte)0);
                            if (length == -1)
                                throw new InvalidDataException(
                                    "Missing terminating null character in UTF-8 text input.");

                            utf8 = utf8.Slice(0, length);
                            while (!utf8.IsEmpty)
                            {
                                var status = Rune.DecodeFromUtf8(utf8, out Rune rune, out int consumed);
                                if (status != OperationStatus.Done)
                                {
                                    // This should never occur if SDL gives use valid data.
                                    throw new InvalidDataException("Failed to decode UTF-8 text input: " + status);
                                }
                                utf8 = utf8.Slice(consumed);

                                var nkey = KeyboardUtil.ToXna(rune.Value, out var key) ? key : (Keys?)null;
                                _window.OnTextInput(new TextInputEventArgs(rune, nkey));
                            }
                        }
                        break;

                    #endregion

                    #region Drop

                    case SDL.EventType.DropBegin:
                        if (_fileDropList == null)
                            _fileDropList = new List<string>();
                        break;

                    case SDL.EventType.DropFile:
                        try
                        {
                            if (_window.IsFilesDroppedHandled)
                            {
                                string filePath = InteropHelpers.Utf8ToString(ev.Drop.File);
                                _fileDropList!.Add(filePath);
                            }
                        }
                        finally
                        {
                            SDL.Free(ev.Drop.File);
                        }
                        break;

                    case SDL.EventType.DropCompleted:
                        if (_fileDropList!.Count > 0)
                        {
                            _window.OnFilesDropped(_fileDropList);
                            _fileDropList = null;
                        }
                        break;

                    #endregion

                    #region Touch

                        // TODO:

                    case SDL.EventType.FingerDown:
                        // ev.TouchFinger;
                        break;

                    case SDL.EventType.FingerUp:
                        // ev.TouchFinger;
                        break;

                    case SDL.EventType.FingerMotion:
                        // ev.TouchFinger;
                        break;
                            
                    case SDL.EventType.MultiGesture:
                        // ev.TouchMultiGesture
                        break;

                    #endregion

                    #region Window

                    case SDL.EventType.WindowEvent:
                        switch (ev.Window.EventId)
                        {
                            case SDL.Window.EventId.Resized:
                            case SDL.Window.EventId.SizeChanged:
                                _window.ClientResize(ev.Window.Data1, ev.Window.Data2);
                                break;

                            case SDL.Window.EventId.FocusGained:
                                IsActive = true;
                                SDL.DisableScreenSaver();
                                break;

                            case SDL.Window.EventId.FocusLost:
                                IsActive = false;
                                SDL.EnableScreenSaver();
                                break;

                            case SDL.Window.EventId.Moved:
                                _window.Moved();
                                break;

                            case SDL.Window.EventId.Close:
                                Interlocked.Increment(ref _isExiting);
                                break;
                        }
                        break;

                        #endregion
                }
            }
        }

        public override void StartRunLoop()
        {
            throw new NotSupportedException("The desktop platform does not support asynchronous run loops.");
        }

        public override void Exit()
        {
            Interlocked.Increment(ref _isExiting);
        }

        public override bool BeforeUpdate(GameTime gameTime)
        {
            return true;
        }

        public override bool BeforeDraw(GameTime gameTime)
        {
            return true;
        }

        public override void EnterFullScreen()
        {
        }

        public override void ExitFullScreen()
        {
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            _window.BeginScreenDeviceChange(willBeFullScreen);
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            _window.EndScreenDeviceChange(screenDeviceName, clientWidth, clientHeight);
        }

        public override void Log(string message)
        {
            Debug.WriteLine(message);
        }

        public override void Present()
        {
            if (Game.GraphicsDevice != null)
                Game.GraphicsDevice.Present();
        }

        protected override void Dispose(bool disposing)
        {
            if (_window != null)
            {
                _window.Dispose();

                Joystick.CloseDevices();
                SDL.Quit();
            }

            base.Dispose(disposing);
        }
    }
}
