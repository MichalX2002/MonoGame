// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Utilities;

namespace MonoGame.Framework
{
    internal class SdlGamePlatform : GamePlatform
    {
        public override GameRunBehavior DefaultRunBehavior => GameRunBehavior.Synchronous;

        private readonly Game _game;
        private readonly List<Keys> _keys;

        private int _isExiting;
        private SdlGameWindow _window;

        public SdlGamePlatform(Game game) : base(game)
        {
            _game = game;
            _keys = new List<Keys>();
            Keyboard.SetKeyList(_keys);

            Sdl.GetVersion(out Sdl.Version sdlVersion);
            Sdl.Major = sdlVersion.Major;
            Sdl.Minor = sdlVersion.Minor;
            Sdl.Patch = sdlVersion.Patch;

            int version = 100 * Sdl.Major + 10 * Sdl.Minor + Sdl.Patch;
            if (version <= 204)
                Debug.WriteLine("Please use SDL 2.0.5 or higher.");

            // Needed so VS can debug the project on Windows
            if (version >= 205 && CurrentPlatform.OS == OS.Windows && Debugger.IsAttached)
                Sdl.SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");

            Sdl.Init((int)(
                Sdl.InitFlags.Video |
                Sdl.InitFlags.Joystick |
                Sdl.InitFlags.GameController |
                Sdl.InitFlags.Haptic
            ));

            Sdl.DisableScreenSaver();

            GamePad.InitDatabase();
            Window = _window = new SdlGameWindow(_game);
        }

        public override void BeforeInitialize()
        {
            SdlRunLoop();

            base.BeforeInitialize();
        }

        protected override void OnIsMouseVisibleChanged()
        {
            _window.SetCursorVisible(_game.IsMouseVisible);
        }

        internal override void OnPresentationChanged(PresentationParameters pp)
        {
            var displayIndex = Sdl.Window.GetDisplayIndex(Window.Handle);
            var displayName = Sdl.Display.GetDisplayName(displayIndex);
            BeginScreenDeviceChange(pp.IsFullScreen);
            EndScreenDeviceChange(displayName, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public override void RunLoop()
        {
            Sdl.Window.Show(Window.Handle);
            InitTaskbarList();

            while (true)
            {
                SdlRunLoop();
                Keyboard.Modifiers = Sdl.Keyboard.GetModState();
                Game.Tick();
                Threading.Run();
                GraphicsDevice.DisposeContexts();

                if (_isExiting > 0)
                    break;
            }
        }

        private void InitTaskbarList()
        {
            if (CurrentPlatform.OS == OS.Windows)
            {
                using (var process = Process.GetCurrentProcess())
                    _window.InitTaskbarList(process.MainWindowHandle);
            }
        }

        private unsafe void SdlRunLoop() 
        { 
            while (Sdl.PollEvent(out Sdl.Event ev) == 1)
            {
                switch (ev.Type)
                {
                    case Sdl.EventType.Quit:
                        _isExiting++;
                        break;

                    case Sdl.EventType.JoyDeviceAdded:
                        Joystick.AddDevice(ev.JoystickDevice.Which);
                        break;

                    case Sdl.EventType.JoyDeviceRemoved:
                        Joystick.RemoveDevice(ev.JoystickDevice.Which);
                        break;

                    case Sdl.EventType.ControllerDeviceRemoved:
                        GamePad.RemoveDevice(ev.ControllerDevice.Which);
                        break;

                    case Sdl.EventType.ControllerButtonUp:
                    case Sdl.EventType.ControllerButtonDown:
                    case Sdl.EventType.ControllerAxisMotion:
                        GamePad.UpdatePacketInfo(ev.ControllerDevice.Which, ev.ControllerDevice.TimeStamp);
                        break;

                    case Sdl.EventType.MouseWheel:
                        const int wheelDelta = 120;
                        Mouse.ScrollY += ev.Wheel.Y * wheelDelta;
                        Mouse.ScrollX += ev.Wheel.X * wheelDelta;
                        break;

                    case Sdl.EventType.MouseMotion:
                        Window.MouseState.X = ev.Motion.X;
                        Window.MouseState.Y = ev.Motion.Y;
                        break;

                    case Sdl.EventType.KeyDown:
                    {
                        var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                        if (!_keys.Contains(key))
                            _keys.Add(key);
                        char character = (char)ev.Key.Keysym.Sym;
                        _window.OnKeyDown(new KeyInputEvent(key));
                        if (char.IsControl(character))
                            _window.OnTextInput(new TextInputEvent(character, key));
                        break;
                    }

                    case Sdl.EventType.KeyUp:
                    {
                        var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                        _keys.Remove(key);
                        _window.OnKeyUp(new KeyInputEvent(key));
                        break;
                    }

                    case Sdl.EventType.TextInput:
                        if (_window.IsTextInputHandled)
                            ProcessTextInput(ev.Text.Text);
                        break;

                    case Sdl.EventType.WindowEvent:

                        switch (ev.Window.EventId)
                        {
                            case Sdl.Window.EventId.Resized:
                            case Sdl.Window.EventId.SizeChanged:
                                _window.ClientResize(ev.Window.Data1, ev.Window.Data2);
                                break;

                            case Sdl.Window.EventId.FocusGained:
                                IsActive = true;
                                break;

                            case Sdl.Window.EventId.FocusLost:
                                IsActive = false;
                                break;

                            case Sdl.Window.EventId.Moved:
                                _window.Moved();
                                break;

                            case Sdl.Window.EventId.Close:
                                _isExiting++;
                                break;
                        }
                        break;
                }
            }
        }

        private unsafe void ProcessTextInput(byte* text)
        {
            int len = 0;
            int utf8character = 0; // using an int to encode multibyte characters longer than 2 bytes
            int charByteSize = 0; // UTF8 char lenght to decode
            int remainingShift = 0;

            byte currentByte;
            while ((currentByte = Marshal.ReadByte((IntPtr)text, len)) != 0)
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

                    // SDL returns UTF8-encoded characters while C# char type is UTF16-encoded (and limited to the 0-FFFF range / does not support surrogate pairs)
                    // so we need to convert it to Unicode codepoint and check if it's within the supported range
                    int codepoint = UTF8ToUnicode(utf8character);

                    if (codepoint >= 0 && codepoint < 0xFFFF)
                    {
                        _window.OnTextInput(new TextInputEvent((char)codepoint, KeyboardUtil.ToXna(codepoint)));
                        // UTF16 characters beyond 0xFFFF are not supported (and would require a surrogate encoding that is not supported by the char type)
                    }
                }

                len++;
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
                return (byte1 % 0x20) * 0x40 + (byte2 % 0x40);
            else if (byte1 < 0xF0 && byte2 >= 0x80 && byte2 < 0xC0 && byte3 >= 0x80 && byte3 < 0xC0)
                return (byte1 % 0x10) * 0x40 * 0x40 + (byte2 % 0x40) * 0x40 + (byte3 % 0x40);
            else if (byte1 < 0xF8 && byte2 >= 0x80 && byte2 < 0xC0 && byte3 >= 0x80 && byte3 < 0xC0 && byte4 >= 0x80 && byte4 < 0xC0)
                return (byte1 % 0x8) * 0x40 * 0x40 * 0x40 + (byte2 % 0x40) * 0x40 * 0x40 + (byte3 % 0x40) * 0x40 + (byte4 % 0x40);
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
                Sdl.Quit();
            }

            base.Dispose(disposing);
        }
    }
}
