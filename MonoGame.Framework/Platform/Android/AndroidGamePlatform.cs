// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework
{
    class AndroidGamePlatform : GamePlatform
    {
        private bool _initialized;
        private AndroidGameWindow _gameWindow;

        public static bool IsPlayingVideo { get; set; }

        public AndroidGamePlatform(Game game) : base(game)
        {
            var activity = AndroidGameActivity.Instance;

            System.Diagnostics.Debug.Assert(
                activity != null, "Must set Game.Activity before creating the Game instance");

            activity.Paused += Activity_Paused;
            activity.Resumed += Activity_Resumed;

            _gameWindow = new AndroidGameWindow(activity, game);
            Window = _gameWindow;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                AndroidGameActivity.Instance.Paused -= Activity_Paused;
                AndroidGameActivity.Instance.Resumed -= Activity_Resumed;
            }
            base.Dispose(disposing);
        }

        public override void Exit()
        {
            AndroidGameActivity.Instance.MoveTaskToBack(true);
        }

        public override void RunLoop()
        {
            throw new NotSupportedException(
                "The Android platform does not support synchronous run loops");
        }

        public override void StartRunLoop()
        {
            _gameWindow.GameView.Resume();
        }

        public override bool BeforeUpdate(GameTime gameTime)
        {
            if (!_initialized)
            {
                Game.DoInitialize();
                _initialized = true;
            }

            return true;
        }

        public override bool BeforeDraw(GameTime gameTime)
        {
            PrimaryThreadLoader.DoLoads();
            return !IsPlayingVideo;
        }

        public override void BeforeInitialize()
        {
            var currentOrientation = AndroidCompatibility.GetAbsoluteOrientation();

            switch (AndroidGameActivity.Instance.Resources.Configuration.Orientation)
            {
                case Android.Content.Res.Orientation.Portrait:
                    _gameWindow.SetOrientation(
                        currentOrientation == DisplayOrientation.PortraitDown 
                        ? DisplayOrientation.PortraitDown 
                        : DisplayOrientation.Portrait,
                        false);
                    break;

                default:
                    _gameWindow.SetOrientation(
                        currentOrientation == DisplayOrientation.LandscapeRight 
                        ? DisplayOrientation.LandscapeRight 
                        : DisplayOrientation.LandscapeLeft,
                        false);
                    break;
            }
            base.BeforeInitialize();
            _gameWindow.GameView.TouchEnabled = true;
        }

        public override bool BeforeRun()
        {
            // Run it as fast as we can to allow for more response on threaded GPU resource creation
            _gameWindow.GameView.Run();

            return false;
        }

        public override void EnterFullScreen()
        {
        }

        public override void ExitFullScreen()
        {
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            // Force the Viewport to be correctly set
            Game.GraphicsDeviceManager.ResetClientBounds();
        }

        // EnterForeground
        void Activity_Resumed(AndroidGameActivity activity)
        {
            if (!IsActive)
            {
                IsActive = true;
                _gameWindow.GameView.Resume();

                if (!_gameWindow.GameView.IsFocused)
                    _gameWindow.GameView.RequestFocus();
            }
        }

        // EnterBackground
        void Activity_Paused(AndroidGameActivity activity)
        {
            if (IsActive)
            {
                IsActive = false;
                _gameWindow.GameView.Pause();
                _gameWindow.GameView.ClearFocus();
            }
        }

        public override GameRunBehavior DefaultRunBehavior
        {
            get { return GameRunBehavior.Asynchronous; }
        }

        public override void Log(string Message)
        {
#if LOGGING
            Android.Util.Log.Debug("MonoGameDebug", Message);
#endif
        }

        public override void Present()
        {
            try
            {
                var device = Game.GraphicsDevice;
                if (device != null)
                    device.Present();

                _gameWindow.GameView.SwapBuffers();
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("Error in swap buffers", ex.ToString());
            }
        }
    }
}
