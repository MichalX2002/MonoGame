// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Utilities;
using System.Text;

namespace Microsoft.Xna.Framework
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

        private void SdlRunLoop()
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

                    case Sdl.EventType.MouseMotion:
                        Window.MouseState.X = ev.Motion.X;
                        Window.MouseState.Y = ev.Motion.Y;
                        break;

                    case Sdl.EventType.MouseWheel:
                        const int wheelDelta = 120;
                        Mouse.ScrollY += ev.Wheel.Y * wheelDelta;
                        Mouse.ScrollX += ev.Wheel.X * wheelDelta;
                        break;

                    case Sdl.EventType.KeyDown:
                        Keys key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                        if (!_keys.Contains(key))
                            _keys.Add(key);

                        if (char.IsControl((char)ev.Key.Keysym.Sym))
                            _window.CallTextInput(ev.Key.Keysym.Sym, key);
                        break;

                    case Sdl.EventType.KeyUp:
                        _keys.Remove(KeyboardUtil.ToXna(ev.Key.Keysym.Sym));
                        break;

                    case Sdl.EventType.TextInput:
                        if (!_window.IsTextInputHandled)
                            break;
                        ProcessTextInputEvent(ev.Text);
                        break;

                    case Sdl.EventType.WindowEvent:
                        if (ev.Window.WindowID != _window.ID)
                            break;
                        ProcessWindowEvent(ev.Window);
                        break;
                }
            }
        }

        private unsafe static int GetCharacter32(char* chars, ref int offset, int count)
        {
            char firstChar = chars[offset];
            return char.IsHighSurrogate(firstChar) && ++offset < count
                ? char.ConvertToUtf32(firstChar, chars[offset])
                : firstChar;
        }

        private unsafe void ProcessTextInputEvent(Sdl.Keyboard.TextInputEvent inputEvent)
        {
            int len = 0;
            while (Marshal.ReadByte((IntPtr)inputEvent.Text, len) != 0)
                len++;

            int charCount = Encoding.UTF8.GetCharCount(inputEvent.Text, len);
            char* chars = stackalloc char[charCount];
            int decodedChars = Encoding.UTF8.GetChars(inputEvent.Text, len, chars, charCount);

            for (int i = 0; i < decodedChars; i++)
            {
                int character = GetCharacter32(chars, ref i, decodedChars);
                _window.CallTextInput(character, KeyboardUtil.ToXna(character));
            }
        }

        private void ProcessWindowEvent(Sdl.Window.Event windowEvent)
        {
            switch (windowEvent.EventID)
            {
                case Sdl.Window.EventId.Resized:
                case Sdl.Window.EventId.SizeChanged:
                    _window.ClientResize(windowEvent.Data1, windowEvent.Data2);
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
