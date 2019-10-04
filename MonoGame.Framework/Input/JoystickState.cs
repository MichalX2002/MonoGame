// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Linq;
using System.Text;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Describes current joystick state.
    /// </summary>
    public struct JoystickState : IEquatable<JoystickState>
    {
        /// <summary>
        /// Gets a value indicating whether the joystick is connected.
        /// </summary>
        public bool IsConnected { get; internal set; }

        /// <summary>
        /// Gets the joystick axis values.
        /// </summary>
        public int[] Axes { get; internal set; }

        /// <summary>
        /// Gets the joystick button values.
        /// </summary>
        public ButtonState[] Buttons { get; internal set; }

        /// <summary>
        /// Gets the joystick hat values.
        /// </summary>
        public JoystickHat[] Hats{ get; internal set; }

        #region Equals

        public static bool operator ==(in JoystickState a, in JoystickState b)
        {
            return a.IsConnected == b.IsConnected
                && a.Axes.SequenceEqual(b.Axes)
                && a.Buttons.SequenceEqual(b.Buttons)
                && a.Hats.SequenceEqual(b.Hats);
        }

        public static bool operator !=(in JoystickState a, in JoystickState b) => !(a == b);

        public bool Equals(JoystickState other) => this == other;
        public override bool Equals(object obj) => obj is JoystickState other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Returns the hash code of the <see cref="JoystickState"/>.
        /// </summary>
        public override int GetHashCode()
        {
            if (!IsConnected)
                return 0;
            unchecked
            {
                int code = 17;
                foreach (int axis in Axes)
                    code = code * 23 + axis;

                foreach (var button in Buttons)
                    code = code * 23 + button.GetHashCode();

                foreach (var hat in Hats)
                    code = code * 23 + hat.GetHashCode();
                return code;
            }
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="JoystickState"/>.
        /// </summary>
        public override string ToString()
        {
            var ret = new StringBuilder(54 - 2 + Axes.Length * 7 + Buttons.Length + Hats.Length * 5);
            ret.Append("[JoystickState: IsConnected=" + (IsConnected ? 1 : 0));

            if (IsConnected)
            {
                ret.Append(", Axes=");
                foreach (var axis in Axes)
                    ret.Append((axis > 0 ? "+" : "") + axis.ToString("00000") + " ");
                ret.Length--;

                ret.Append(", Buttons=");
                foreach (var button in Buttons)
                    ret.Append((int)button);

                ret.Append(", Hats=");
                foreach (var hat in Hats)
                    ret.Append(hat + " ");
                ret.Length--;
            }

            ret.Append("]");
            return ret.ToString();
        }

        #endregion
    }
}

