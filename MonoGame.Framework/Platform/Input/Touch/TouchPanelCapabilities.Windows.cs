using System.Runtime.InteropServices;

namespace MonoGame.Framework.Input.Touch
{
    public readonly partial struct TouchPanelCapabilities
    {
        private const int SM_MAXIMUMTOUCHES = 95;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern int GetSystemMetrics(int nIndex);

        internal static bool PlatformGetIsConnected(GameWindow window)
        {
            int maxTouchCount = PlatformGetMaxTouchCount(window);
            return maxTouchCount > 0;
        }

        internal static int PlatformGetMaxTouchCount(GameWindow window)
        {
            return GetSystemMetrics(SM_MAXIMUMTOUCHES);
        }
    }
}
