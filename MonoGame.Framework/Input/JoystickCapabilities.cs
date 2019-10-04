// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Describes joystick capabilities.
    /// </summary>
    public struct JoystickCapabilities : IEquatable<JoystickCapabilities>
    {
        /// <summary>
        /// Gets a value indicating whether the joystick is connected.
        /// </summary>
        public bool IsConnected { get; internal set; }

        /// <summary>
        /// Gets the unique identifier of the joystick.
        /// </summary>
        public string Identifier { get; internal set; }

        /// <summary>
        /// Gets a value indicating if the joystick is a gamepad.
        /// </summary>
        public bool IsGamepad { get; internal set; }

        /// <summary>
        /// Gets the axis count.
        /// </summary>
        public int AxisCount { get; internal set; }

        /// <summary>
        /// Gets the button count.
        /// </summary>
        public int ButtonCount { get; internal set; }

        /// <summary>
        /// Gets the hat count.
        /// </summary>
        public int HatCount { get; internal set; }

        #region Equals

        public static bool operator ==(in JoystickCapabilities a, in JoystickCapabilities b)
        {
            return a.IsConnected == b.IsConnected
                && a.Identifier == b.Identifier
                && a.IsGamepad == b.IsGamepad
                && a.AxisCount == b.AxisCount
                && a.ButtonCount == b.ButtonCount
                && a.HatCount == b.HatCount;
        }
        
        public static bool operator !=(in JoystickCapabilities a, in JoystickCapabilities b) => !(a == b);

        public bool Equals(JoystickCapabilities other) => this == other;
        public override bool Equals(object obj) => obj is JoystickCapabilities other && Equals(other);

        #endregion

        /// <summary>
        /// Serves as a hash function for a <see cref="JoystickCapabilities"/> object.
        /// </summary>
        public override int GetHashCode() => Identifier.GetHashCode();

        /// <summary>
        /// Returns a string that represents the current <see cref="JoystickCapabilities"/>.
        /// </summary>
        public override string ToString() => 
            "[JoystickCapabilities: IsConnected=" + IsConnected + 
            ", Identifier=" + Identifier + 
            ", IsGamepad=" + IsGamepad + 
            " , AxisCount=" + AxisCount +
            ", ButtonCount=" + ButtonCount +
            ", HatCount=" + HatCount +
            "]";
    }
}

