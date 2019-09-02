// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Input
{
    public readonly struct GamePadDPad
    {
        /// <summary>
        /// Gets a value indicating wethever down is pressed on the directional pad.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the down button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public readonly ButtonState Down;

        /// <summary>
        /// Gets a value indicating wethever left is pressed on the directional pad.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the left button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public readonly ButtonState Left;

        /// <summary>
        /// Gets a value indicating wethever right is pressed on the directional pad.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the right button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public readonly ButtonState Right;

        /// <summary>
        /// Gets a value indicating wethever up is pressed on the directional pad.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the up button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public readonly ButtonState Up;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadDPad"/> struct.
        /// </summary>
        /// <param name="upValue">Current state of directional pad up.</param>
        /// <param name="downValue">Current state of directional pad down.</param>
        /// <param name="leftValue">Current state of directional pad left.</param>
        /// <param name="rightValue">Current state of directional pad right.</param>
        public GamePadDPad(ButtonState upValue, ButtonState downValue, ButtonState leftValue, ButtonState rightValue) : this()
        {
            Up = upValue;
            Down = downValue;
            Left = leftValue;
            Right = rightValue;
        }

        internal GamePadDPad(Buttons[] buttons) : this()
        {
            foreach (var button in buttons)
                ConvertButtonToDirection(button, ref Down, ref Left, ref Right, ref Up);
        }

        internal GamePadDPad(Buttons button) : this()
        {
            ConvertButtonToDirection(button, ref Down, ref Left, ref Right, ref Up);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadDPad"/> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(in GamePadDPad left, in GamePadDPad right)
        {
            return (left.Down == right.Down)
                && (left.Left == right.Left)
                && (left.Right == right.Right)
                && (left.Up == right.Up);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadDPad"/> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(in GamePadDPad left, in GamePadDPad right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>
        /// true if <paramref name="obj"/> is a <see cref="GamePadDPad"/> and 
        /// has the same value as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is GamePadDPad other) && (this == other);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="GamePadDPad"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return
                (Down == ButtonState.Pressed ? 1 : 2) +
                (Left == ButtonState.Pressed ? 4 : 8) +
                (Right == ButtonState.Pressed ? 16 : 32) +
                (Up == ButtonState.Pressed ? 64 : 128);
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="GamePadDPad"/>
        /// in a format of 0000 where each number represents a boolean value of each respecting object property: Left, Up, Right, Down.
        /// </summary>
        /// <returns>A string that represents the current <see cref="GamePadDPad"/>.</returns>
        public override string ToString()
        {
            return "" + (int)Left + (int)Up + (int)Right + (int)Down;
        }

        private void ConvertButtonToDirection(Buttons button,
            ref ButtonState down, ref ButtonState left, 
            ref ButtonState right, ref ButtonState up)
        {
            if ((button & Buttons.DPadDown) == Buttons.DPadDown)
                down = ButtonState.Pressed;

            if ((button & Buttons.DPadLeft) == Buttons.DPadLeft)
                left = ButtonState.Pressed;

            if ((button & Buttons.DPadRight) == Buttons.DPadRight)
                right = ButtonState.Pressed;

            if ((button & Buttons.DPadUp) == Buttons.DPadUp)
                up = ButtonState.Pressed;
        }
    }
}
