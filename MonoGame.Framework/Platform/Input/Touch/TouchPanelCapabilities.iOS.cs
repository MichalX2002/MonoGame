using UIKit;

namespace MonoGame.Framework.Input.Touch
{
    public readonly partial struct TouchPanelCapabilities
    {
        internal static bool PlatformGetIsConnected(GameWindow window)
        {
            return true;
        }

        internal static int PlatformGetMaxTouchCount(GameWindow window)
        {
            // iPhone supports 5, iPad 11
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return 5;
            else
                return 11;
        }
    }
}
