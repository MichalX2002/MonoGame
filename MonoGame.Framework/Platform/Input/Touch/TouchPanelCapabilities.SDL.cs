
namespace MonoGame.Framework.Input.Touch
{
    public readonly partial struct TouchPanelCapabilities
    {
        internal static bool PlatformGetIsConnected(GameWindow window)
        {
            return SDL.Touch.SDL_GetNumTouchDevices() > 0;
        }

        internal static int PlatformGetMaxTouchCount(GameWindow window)
        {
            return PlatformGetIsConnected(window) ? 4 : 0;
        }
    }
}
