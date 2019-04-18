// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
#if ANDROID
using Android.Content.PM;
#endif
#if IOS
using UIKit;
#endif


namespace Microsoft.Xna.Framework.Input.Touch
{
    /// <summary>
    /// Allows retrieval of capabilities information from touch panel device.
    /// </summary>
    public struct TouchPanelCapabilities
    {
        private bool isConnected;
        private bool initialized;

        internal void Initialize()
        {
            if (!initialized)
            {
                initialized = true;

                // There does not appear to be a way of finding out if a touch device supports pressure.
                // XNA does not expose a pressure value, so let's assume it doesn't support it.
                HasPressure = false;

#if WINDOWS_UAP
                // Is a touch device present?
                // Iterate through all pointer devices and find the maximum number of concurrent touches possible
                MaximumTouchCount = 0;
                var pointerDevices = Windows.Devices.Input.PointerDevice.GetPointerDevices();
                foreach (var pointerDevice in pointerDevices)
                {
                    MaximumTouchCount = Math.Max(MaximumTouchCount, (int)pointerDevice.MaxContacts);

                    if (pointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                        isConnected = true;
                }
#elif WINDOWS
                MaximumTouchCount = GetSystemMetrics(SM_MAXIMUMTOUCHES);
                isConnected = (MaximumTouchCount > 0);
#elif ANDROID
                // http://developer.android.com/reference/android/content/pm/PackageManager.html#FEATURE_TOUCHSCREEN
                var pm = Game.Activity.PackageManager;
                isConnected = pm.HasSystemFeature(PackageManager.FeatureTouchscreen);
                if (pm.HasSystemFeature(PackageManager.FeatureTouchscreenMultitouchJazzhand))
                    MaximumTouchCount = 5;
                else if (pm.HasSystemFeature(PackageManager.FeatureTouchscreenMultitouchDistinct))
                    MaximumTouchCount = 2;
                else
                    MaximumTouchCount = 1;
#elif IOS
                //iPhone supports 5, iPad 11
                isConnected = true;
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                    MaximumTouchCount = 5;
                else //Pad
                    MaximumTouchCount = 11;
#else
                //Touch isn't implemented in OpenTK, so no linux or mac https://github.com/opentk/opentk/issues/80
                isConnected = false;
#endif
            }
        }

        public bool HasPressure { get; private set; }

        /// <summary>
        /// Returns true if a device is available for use.
        /// </summary>
        public bool IsConnected => isConnected;

        /// <summary>
        /// Returns the maximum number of touch locations tracked by the touch panel device.
        /// </summary>
        public int MaximumTouchCount { get; private set; }

#if WINDOWS
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        static extern int GetSystemMetrics(int nIndex);

        const int SM_MAXIMUMTOUCHES = 95;
#endif
    }
}