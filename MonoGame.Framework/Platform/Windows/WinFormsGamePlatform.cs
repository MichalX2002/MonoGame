// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Media;

namespace MonoGame.Framework
{
    class WinFormsGamePlatform : GamePlatform
    {
        //internal static string LaunchParameters;

        private WinFormsGameWindow _window;

        public override GameWindow Window => _window;

        public WinFormsGamePlatform(Game game) : base(game)
        {
            _window = new WinFormsGameWindow(this);
        }

        public override GameRunBehavior DefaultRunBehavior => GameRunBehavior.Synchronous;

        protected override void OnIsMouseVisibleChanged()
        {
            _window.MouseVisibleToggled();
        }

        public override bool BeforeRun()
        {
            _window.UpdateWindows();
            return base.BeforeRun();
        }

        public override void BeforeInitialize()
        {
            base.BeforeInitialize();
            
            if (Game.GraphicsDeviceManager == null)
            {
                _window.Initialize(
                    GraphicsDeviceManager.DefaultBackBufferWidth,
                    GraphicsDeviceManager.DefaultBackBufferHeight);
            }
            else
            {
                var pp = Game.GraphicsDevice.PresentationParameters;
                _window.Initialize(pp);
            }
        }

        public override void RunLoop()
        {
            _window.RunLoop();
        }

        public override void StartRunLoop()
        {
            throw new NotSupportedException(
                "The Windows platform does not support asynchronous run loops.");
        }
        
        public override void Exit()
        {
            if (_window != null)
            {
                _window.Dispose();
                _window = null!;
            }
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

        internal override void OnPresentationChanged(PresentationParameters pp)
        {
            _window.OnPresentationChanged(pp);
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
        }

        public override void Log(string message)
        {
            Debug.WriteLine(message);
        }

        public override void Present()
        {
            var device = Game.GraphicsDevice;
            if ( device != null )
                device.Present();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_window != null)
                {
                    _window.Dispose();
                    _window = null!;
                }
                MediaManagerState.CheckShutdown();
            }

            base.Dispose(disposing);
        }
    }
}
