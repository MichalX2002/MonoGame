// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Input
{
    static partial class Joystick
    {
        private const bool PlatformIsSupported = false;

        private static JoystickCapabilities PlatformGetCapabilities(int index)
        {
            return JoystickCapabilities.Default;
        }

        private static JoystickState PlatformGetState(int index)
        {
            return JoystickState.Default;
        }

        private static int PlatformLastConnectedIndex => -1;

        private static void PlatformGetState(ref JoystickState joystickState, int index)
        {
        }
    }
}

