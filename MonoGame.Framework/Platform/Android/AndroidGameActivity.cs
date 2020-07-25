// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;

namespace MonoGame.Framework
{
    // TODO: remove static game activity instance and make it more hierarchical

    [CLSCompliant(false)]
    public class AndroidGameActivity : Activity
    {
        public static AndroidGameActivity Instance { get; private set; }

        public Game Game { get; internal set; }

        private ScreenReceiver screenReceiver;
        private OrientationListener _orientationListener;

        public event Event<AndroidGameActivity>? Paused;
        public event Event<AndroidGameActivity>? Resumed;

        public bool AutoPauseAndResumeMediaPlayer = true;
        public bool RenderOnUIThread = true;

        /// <summary>
        /// OnCreate called when the activity is launched from cold or after the app
        /// has been killed due to a higher priority app needing the memory.
        /// </summary>
        /// <param name='savedInstanceState'>Saved instance state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            var filter = new IntentFilter();
            filter.AddAction(Intent.ActionScreenOff);
            filter.AddAction(Intent.ActionScreenOn);
            filter.AddAction(Intent.ActionUserPresent);

            screenReceiver = new ScreenReceiver();
            RegisterReceiver(screenReceiver, filter);

            _orientationListener = new OrientationListener(this);
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            // we need to refresh the viewport here.
            base.OnConfigurationChanged(newConfig);
        }

        protected override void OnPause()
        {
            base.OnPause();
            Paused?.Invoke(this);

            if (_orientationListener.CanDetectOrientation())
                _orientationListener.Disable();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Resumed?.Invoke(this);

            if (Game != null)
            {
                var deviceManager = Game.Services.GetService<IGraphicsDeviceManager>();
                if (deviceManager == null)
                    return;

                ((GraphicsDeviceManager)deviceManager).ForceSetFullScreen();
                ((AndroidGameWindow)Game.Window).GameView.RequestFocus();
                if (_orientationListener.CanDetectOrientation())
                    _orientationListener.Enable();
            }
        }

        protected override void OnDestroy()
        {
            UnregisterReceiver(screenReceiver);
            ScreenReceiver.ScreenLocked = false;
            _orientationListener = null;

            Game?.Dispose();
            Game = null;

            base.OnDestroy();
        }
    }

    [CLSCompliant(false)]
    public static class ActivityExtensions
    {
        public static ActivityAttribute GetActivityAttribute(this AndroidGameActivity obj)
        {
            var attr = obj.GetType().GetCustomAttributes(typeof(ActivityAttribute), true);
            if (attr == null)
                return null;
            
            return (ActivityAttribute)attr[0];
        }
    }

}
