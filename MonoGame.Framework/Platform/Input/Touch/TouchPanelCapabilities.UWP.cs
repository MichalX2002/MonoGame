
namespace MonoGame.Framework.Input.Touch
{
    public readonly partial struct TouchPanelCapabilities
    {
        internal static bool PlatformGetIsConnected(GameWindow window)
        {
            // Is a touch device present?
            var pointerDevices = Windows.Devices.Input.PointerDevice.GetPointerDevices();
            foreach (var pointerDevice in pointerDevices)
            {
                if (pointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    return true;
            }
            return false;
        }

        internal static int PlatformGetMaxTouchCount(GameWindow window)
        {
            // Iterate through all pointer devices and find the maximum number of concurrent touches possible
            int maximumTouchCount = 0;
            var pointerDevices = Windows.Devices.Input.PointerDevice.GetPointerDevices();
            foreach (var pointerDevice in pointerDevices)
            {
                if (pointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    maximumTouchCount = Math.Max(maximumTouchCount, (int)pointerDevice.MaxContacts);
            }
            return maximumTouchCount;
        }
    }
}
