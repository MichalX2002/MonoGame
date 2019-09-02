// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Input
{
    static partial class Joystick
    {
        private const bool PlatformIsSupported = false;

        private static JoystickCapabilities PlatformGetCapabilities(int index)
        {
            return new JoystickCapabilities()
            {
                IsConnected = false,
                IsGamepad = false,
                AxisCount = 0,
                ButtonCount = 0,
                HatCount = 0
            };
        }

        private static JoystickState PlatformGetState(int index)
        {
            return new JoystickState()
            {
                IsConnected = false,
                Axes = Array.Empty<int>(),
                Hats = Array.Empty<JoystickHat>(),
                Buttons = Array.Empty<ButtonState>()
            };
        }
    }
}

