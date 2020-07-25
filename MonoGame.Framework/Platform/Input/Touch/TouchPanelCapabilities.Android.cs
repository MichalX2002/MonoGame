using Android.Content.PM;

namespace MonoGame.Framework.Input.Touch
{
    public readonly partial struct TouchPanelCapabilities
    {
        internal static bool PlatformGetIsConnected(GameWindow window)
        {
            // http://developer.android.com/reference/android/content/pm/PackageManager.html#FEATURE_TOUCHSCREEN
            var pm = AndroidGameActivity.Instance.PackageManager;
            return pm.HasSystemFeature(PackageManager.FeatureTouchscreen);
        }

        internal static int PlatformGetMaxTouchCount(GameWindow window)
        {
            // http://developer.android.com/reference/android/content/pm/PackageManager.html#FEATURE_TOUCHSCREEN
            var pm = AndroidGameActivity.Instance.PackageManager;
            if (pm.HasSystemFeature(PackageManager.FeatureTouchscreenMultitouchJazzhand))
                return 5;
            else if (pm.HasSystemFeature(PackageManager.FeatureTouchscreenMultitouchDistinct))
                return 2;
            else
                return 1;
        }
    }
}
