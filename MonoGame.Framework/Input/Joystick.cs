// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Input
{
    /// <summary> 
    /// Allows interaction with joysticks. Unlike <see cref="GamePad"/> the number of Buttons/Axes/DPads is not limited.
    /// </summary>
    public static partial class Joystick
    {
        /// <summary>
        /// Gets a value indicating whether the current platform supports reading raw joystick data.
        /// </summary>
        /// <value><see langword="true"/> if the current platform supports reading raw joystick data; otherwise, <see langword="false"/>.</value>
        public static bool IsSupported => PlatformIsSupported;

        /// <summary>
        /// Gets a value indicating the last joystick index connected to the system. If this value is less than 0, no joysticks are connected.
        /// <para>The order joysticks are connected and disconnected determines their index.
        /// As such, this value may be larger than 0 even if only one joystick is connected.
        /// </para>
        /// </summary>
        public static int LastConnectedIndex
        {
            get { return PlatformLastConnectedIndex; }
        }

        /// <summary>
        /// Gets the capabilites of the joystick.
        /// </summary>
        /// <param name="index">Index of the joystick you want to access.</param>
        /// <returns>The capabilites of the joystick.</returns>
        public static JoystickCapabilities GetCapabilities(int index)
        {
            return PlatformGetCapabilities(index);
        }

        /// <summary>
        /// Gets the current state of the joystick.
        /// </summary>
        /// <param name="index">Index of the joystick you want to access.</param>
        /// <returns>The state of the joystick.</returns>
        public static JoystickState GetState(int index)
        {
            return PlatformGetState(index);
        }
    }
}
