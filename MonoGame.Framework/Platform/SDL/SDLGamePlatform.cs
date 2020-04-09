// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;

namespace MonoGame.Framework
{
    internal class SdlGamePlatform : GamePlatform
    {
        const int MouseWheelDelta = 120;

        public override GameRunBehavior DefaultRunBehavior => GameRunBehavior.Synchronous;

        private readonly Game _game;
        private readonly List<Keys> _keys;

        private int _isExiting;
        private SdlGameWindow _window;

        public SdlGamePlatform(Game game) : base(game)
        {
            _game = game;
            _keys = new List<Keys>();
            Keyboard.SetKeysDownList(_keys);

            SDL.GetVersion(out SDL.Version version);

            if (version.Major < 2 || version.Minor < 0 || version.Patch < 5)
                Debug.WriteLine("Please use SDL 2.0.5 or higher.");

            // Needed so VS can debug the project on Windows
            if (version.Major >= 2 && version.Minor >= 0 && version.Patch >= 5 &&
                Debugger.IsAttached &&
                PlatformInfo.OS == PlatformInfo.OperatingSystem.Windows)
                SDL.SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");

            SDL.Init((int)(
                SDL.InitFlags.Video |
                SDL.InitFlags.Joystick |
                SDL.InitFlags.GameController |
                SDL.InitFlags.Haptic));

            SDL.DisableScreenSaver();

            GamePad.InitDatabase();
            Window = _window = new SdlGameWindow(_game);
        }

        public override void BeforeInitialize()
        {
            PollSdlEvents();

            base.BeforeInitialize();
        }

        protected override void OnIsMouseVisibleChanged()
        {
            _window.SetCursorVisible(_game.IsMouseVisible);
        }

        internal override void OnPresentationChanged(PresentationParameters pp)
        {
            var displayIndex = SDL.Window.GetDisplayIndex(Window.Handle);
            var displayName = SDL.Display.GetDisplayName(displayIndex);
            BeginScreenDeviceChange(pp.IsFullScreen);
            EndScreenDeviceChange(displayName, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public override void RunLoop()
        {
            SDL.Window.Show(Window.Handle);
            _window.TaskbarList.WindowHandle = _window.GetSubsystemWindowHandle();

            while (true)
            {
                PollSdlEvents();
                Keyboard.Modifiers = SDL.Keyboard.GetModState();

                Game.Tick();

                Threading.Run();
                GraphicsDevice.DisposeContexts();

                if (_isExiting > 0)
                    break;
            }
        }

        private unsafe void PollSdlEvents()
        {
            while (SDL.PollEvent(out SDL.Event ev) == 1)
            {
                switch (ev.Type)
                {
                    case SDL.EventType.Quit:
                        Interlocked.Increment(ref _isExiting);
                        break;

                    case SDL.EventType.JoyDeviceAdded:
                        Joystick.AddDevice(ev.JoystickDevice.Which);
                        break;

                    case SDL.EventType.JoyDeviceRemoved:
                        Joystick.RemoveDevice(ev.JoystickDevice.Which);
                        break;

                    case SDL.EventType.ControllerDeviceRemoved:
                        GamePad.RemoveDevice(ev.ControllerDevice.Which);
                        break;

                    case SDL.EventType.ControllerButtonUp:
                    case SDL.EventType.ControllerButtonDown:
                    case SDL.EventType.ControllerAxisMotion:
                        GamePad.UpdatePacketInfo(ev.ControllerDevice.Which, ev.ControllerDevice.TimeStamp);
                        break;

                    case SDL.EventType.MouseWheel:
                        Mouse.ScrollY += ev.Wheel.Y * MouseWheelDelta;
                        Mouse.ScrollX += ev.Wheel.X * MouseWheelDelta;
                        break;

                    case SDL.EventType.MouseMotion:
                        Window.MouseState.X = ev.Motion.X;
                        Window.MouseState.Y = ev.Motion.Y;
                        break;

                    case SDL.EventType.KeyDown:
                    {
                        var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                        if (!_keys.Contains(key))
                            _keys.Add(key);

                        char character = (char)ev.Key.Keysym.Sym;
                        _window.OnKeyDown(new KeyInputEventArgs(key));

                        if (char.IsControl(character))
                            _window.OnTextInput(new TextInputEventArgs(character, key));
                        break;
                    }

                    case SDL.EventType.KeyUp:
                    {
                        var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                        _keys.Remove(key);
                        _window.OnKeyUp(new KeyInputEventArgs(key));
                        break;
                    }

                    case SDL.EventType.TextInput:
                        if (_window.IsTextInputHandled)
                            ProcessTextInput(ev.Text.Text);
                        break;

                    case SDL.EventType.DropFile:
                        try
                        {
                            _window.InvokeFileDropped(InteropHelpers.Utf8ToString(ev.Drop.File));
                        }
                        finally
                        {
                            SDL.Free(ev.Drop.File);
                        }
                        break;

                    case SDL.EventType.WindowEvent:
                        switch (ev.Window.EventId)
                        {
                            case SDL.Window.EventId.Resized:
                            case SDL.Window.EventId.SizeChanged:
                                _window.ClientResize(ev.Window.Data1, ev.Window.Data2);
                                break;

                            case SDL.Window.EventId.FocusGained:
                                IsActive = true;
                                break;

                            case SDL.Window.EventId.FocusLost:
                                IsActive = false;
                                break;

                            case SDL.Window.EventId.Moved:
                                _window.Moved();
                                break;

                            case SDL.Window.EventId.Close:
                                Interlocked.Increment(ref _isExiting);
                                break;
                        }
                        break;
                }
            }
        }

        private unsafe void ProcessTextInput(byte* text)
        {
            int length = 0;
            int utf8character = 0; // using an int to encode multibyte characters longer than 2 bytes
            int charByteSize = 0; // UTF8 char length to decode
            int remainingShift = 0;

            byte currentByte;
            while ((currentByte = Marshal.ReadByte((IntPtr)text, length)) != 0)
            {
                // we're reading the first UTF8 byte, we need to check if it's multibyte
                if (charByteSize == 0)
                {
                    if (currentByte < 192)
                        charByteSize = 1;
                    else if (currentByte < 224)
                        charByteSize = 2;
                    else if (currentByte < 240)
                        charByteSize = 3;
                    else
                        charByteSize = 4;

                    utf8character = 0;
                    remainingShift = 4;
                }

                // assembling the character
                utf8character <<= 8;
                utf8character |= currentByte;

                charByteSize--;
                remainingShift--;

                if (charByteSize == 0) // finished decoding the current character
                {
                    utf8character <<= remainingShift * 8; // shifting it to full UTF8 scope

                    // SDL returns UTF8-encoded characters while C# char type is UTF16-encoded 
                    // (and limited to the 0-FFFF range / does not support surrogate pairs)
                    // so we need to convert it to Unicode codepoint and check if it's within the supported range
                    int codepoint = UTF8ToUnicode(utf8character);
                    if (codepoint >= 0)
                    {
                        _window.OnTextInput(
                            new TextInputEventArgs(codepoint, KeyboardUtil.ToXna(codepoint)));
                    }
                }

                length++;
            }
        }

        private int UTF8ToUnicode(int utf8)
        {
            int byte4 = utf8 & 0xFF,
                byte3 = (utf8 >> 8) & 0xFF,
                byte2 = (utf8 >> 16) & 0xFF,
                byte1 = (utf8 >> 24) & 0xFF;

            if (byte1 < 0x80)
                return byte1;
            else if (byte1 < 0xC0)
                return -1;
            else if (byte1 < 0xE0 && byte2 >= 0x80 && byte2 < 0xC0)
                return byte1 % 0x20 * 0x40 + (byte2 % 0x40);
            else if (byte1 < 0xF0 && byte2 >= 0x80 && byte2 < 0xC0 && byte3 >= 0x80 && byte3 < 0xC0)
                return byte1 % 0x10 * 0x40 * 0x40 + byte2 % 0x40 * 0x40 + (byte3 % 0x40);
            else if (byte1 < 0xF8 && byte2 >= 0x80 && byte2 < 0xC0 && byte3 >= 0x80 && byte3 < 0xC0 && byte4 >= 0x80 && byte4 < 0xC0)
                return byte1 % 0x8 * 0x40 * 0x40 * 0x40 + byte2 % 0x40 * 0x40 * 0x40 + byte3 % 0x40 * 0x40 + (byte4 % 0x40);
            else
                return -1;
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
                _window = null;

                Joystick.CloseDevices();
                SDL.Quit();
            }

            base.Dispose(disposing);
        }
    }
}
