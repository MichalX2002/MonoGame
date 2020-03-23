// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#if WINDOWS
using System.Runtime.InteropServices;
#endif

#if ANDROID
using Android.Content.PM;
#endif

#if IOS
using UIKit;
#endif

namespace MonoGame.Framework.Input.Touch
{
    /// <summary>
    /// Allows retrieval of capability information from the touch panel device.
    /// </summary>
    public readonly struct TouchPanelCapabilities
    {
#if WINDOWS
        private const int SM_MAXIMUMTOUCHES = 95;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern int GetSystemMetrics(int nIndex);
#endif

        /// <summary>
        /// Gets whether a touch device is available.
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Gets the maximum number of touch locations tracked by the touch panel device.
        /// </summary>
        public int MaximumTouchCount { get; }

        // There does not appear to be a way of finding out if a touch device supports pressure.
        // XNA does not expose a pressure value, so let's assume it doesn't support it.

        /// <summary>
        /// Gets whether the device has pressure sensitivity support.
        /// </summary>
        public bool HasPressure { get; }

        /// <summary>
        /// Constructs the <see cref="TouchPanelCapabilities"/>.
        /// </summary>
        public TouchPanelCapabilities(bool isConnected, int maximumTouchCount, bool hasPressure)
        {
            IsConnected = isConnected;
            MaximumTouchCount = maximumTouchCount;
            HasPressure = hasPressure;
        }

        static internal bool CheckIfConnected()
        {
#if WINDOWS_UAP
            // Is a touch device present?
            var pointerDevices = Windows.Devices.Input.PointerDevice.GetPointerDevices();
            foreach (var pointerDevice in pointerDevices)
            {
                if (pointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    return true;
            }
            return false;
#elif WINDOWS
            int maximumTouchCount = GetSystemMetrics(SM_MAXIMUMTOUCHES);
            return maximumTouchCount > 0;
#elif ANDROID
            // http://developer.android.com/reference/android/content/pm/PackageManager.html#FEATURE_TOUCHSCREEN
            return pm.HasSystemFeature(PackageManager.FeatureTouchscreen);
#elif IOS
            return true;
#else
            //Touch isn't implemented in OpenTK, so no linux or mac https://github.com/opentk/opentk/issues/80
            return false;
#endif
        }

        static internal int GetMaxTouchCount()
        {
#if WINDOWS_UAP
            // Iterate through all pointer devices and find the maximum number of concurrent touches possible
            int maximumTouchCount = 0;
            var pointerDevices = Windows.Devices.Input.PointerDevice.GetPointerDevices();
            foreach (var pointerDevice in pointerDevices)
            {
                if (pointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    maximumTouchCount = Math.Max(maximumTouchCount, (int)pointerDevice.MaxContacts);
            }
            return maximumTouchCount;
#elif WINDOWS
            return GetSystemMetrics(SM_MAXIMUMTOUCHES);
#elif ANDROID
            // http://developer.android.com/reference/android/content/pm/PackageManager.html#FEATURE_TOUCHSCREEN
            var pm = Game.Activity.PackageManager;
            if (pm.HasSystemFeature(PackageManager.FeatureTouchscreenMultitouchJazzhand))
                return 5;
            else if (pm.HasSystemFeature(PackageManager.FeatureTouchscreenMultitouchDistinct))
                return 2;
            else
                return 1;
#elif IOS
            //iPhone supports 5, iPad 11
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return 5;
            else
                return 11;
#else
            //Touch isn't implemented in OpenTK, so no linux or mac https://github.com/opentk/opentk/issues/80
            return 0;
#endif
        }
    }
}