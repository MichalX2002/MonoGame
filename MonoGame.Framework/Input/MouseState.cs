// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Represents a mouse state with cursor position and button press information.
    /// </summary>
    public struct MouseState : IEquatable<MouseState>
    {
        #region Properties

        /// <summary>
        /// Gets horizontal position of the cursor in relation to the window.
        /// </summary>
        public int X { get; internal set; }

        /// <summary>
        /// Gets vertical position of the cursor in relation to the window.
        /// </summary>
		public int Y { get; internal set; }

        /// <summary>
        /// Gets cursor position.
        /// </summary>
        public readonly Point Position => new Point(X, Y);

        /// <summary>
        /// Gets state of the left mouse button.
        /// </summary>
        public ButtonState LeftButton { get; internal set; }

        /// <summary>
        /// Gets state of the middle mouse button.
        /// </summary>
		public ButtonState MiddleButton { get; internal set; }

        /// <summary>
        /// Gets state of the right mouse button.
        /// </summary>
		public ButtonState RightButton { get; internal set; }

        /// <summary>
        /// Gets the cumulative scroll wheel value since the game start.
        /// </summary>
		public int ScrollWheelValue { get; internal set; }

        /// <summary>
        /// Gets the cumulative horizontal scroll wheel value since the game start.
        /// </summary>
        public int HorizontalScrollWheelValue { get; internal set; }

        /// <summary>
        /// Gets state of the XButton1.
        /// </summary>
		public ButtonState XButton1 { get; internal set; }

        /// <summary>
        /// Gets state of the XButton2.
        /// </summary>
		public ButtonState XButton2 { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MouseState.
        /// </summary>
        /// <param name="x">Horizontal position of the mouse in relation to the window.</param>
        /// <param name="y">Vertical position of the mouse in relation to the window.</param>
        /// <param name="scrollWheel">Mouse scroll wheel's value.</param>
        /// <param name="leftButton">Left mouse button's state.</param>
        /// <param name="middleButton">Middle mouse button's state.</param>
        /// <param name="rightButton">Right mouse button's state.</param>
        /// <param name="xButton1">XBUTTON1's state.</param>
        /// <param name="xButton2">XBUTTON2's state.</param>
        /// <remarks>
        /// Normally <see cref="Mouse.GetState()"/> should be used to get mouse current state.
        /// The constructor is provided for simulating mouse input.
        /// </remarks>
        public MouseState(
            int x,
            int y,
            int scrollWheel,
            ButtonState leftButton,
            ButtonState middleButton,
            ButtonState rightButton,
            ButtonState xButton1,
            ButtonState xButton2)
        {
            X = x;
            Y = y;
            ScrollWheelValue = scrollWheel;
            LeftButton = leftButton;
            MiddleButton = middleButton;
            RightButton = rightButton;
            XButton1 = xButton1;
            XButton2 = xButton2;
            HorizontalScrollWheelValue = 0;
        }

        /// <summary>
        /// Initializes a new instance of the MouseState.
        /// </summary>
        /// <param name="x">Horizontal position of the mouse in relation to the window.</param>
        /// <param name="y">Vertical position of the mouse in relation to the window.</param>
        /// <param name="scrollWheel">Mouse scroll wheel's value.</param>
        /// <param name="leftButton">Left mouse button's state.</param>
        /// <param name="middleButton">Middle mouse button's state.</param>
        /// <param name="rightButton">Right mouse button's state.</param>
        /// <param name="xButton1">XBUTTON1's state.</param>
        /// <param name="xButton2">XBUTTON2's state.</param>
        /// <param name="horizontalScrollWheel">Mouse horizontal scroll wheel's value.</param>
        /// <remarks>
        /// Normally <see cref="Mouse.GetState()"/> should be used to get mouse current state.
        /// The constructor is provided for simulating mouse input.
        /// </remarks>
        public MouseState(
            int x,
            int y,
            int scrollWheel,
            ButtonState leftButton,
            ButtonState middleButton,
            ButtonState rightButton,
            ButtonState xButton1,
            ButtonState xButton2,
            int horizontalScrollWheel)
        {
            X = x;
            Y = y;
            ScrollWheelValue = scrollWheel;
            LeftButton = leftButton;
            MiddleButton = middleButton;
            RightButton = rightButton;
            XButton1 = xButton1;
            XButton2 = xButton2;
            HorizontalScrollWheelValue = horizontalScrollWheel;
        }

        #endregion

        #region Equals

        public static bool operator ==(in MouseState a, in MouseState b)
		{
			return a.X == b.X &&
                   a.Y == b.Y &&
                   a.LeftButton == b.LeftButton &&
                   a.MiddleButton == b.MiddleButton &&
                   a.RightButton == b.RightButton &&
                   a.ScrollWheelValue == b.ScrollWheelValue &&
                   a.HorizontalScrollWheelValue == b.HorizontalScrollWheelValue &&
                   a.XButton1 == b.XButton1 &&
                   a.XButton2 == b.XButton2;
		}

        public static bool operator !=(in MouseState a, in MouseState b) => !(a == b);

        public readonly bool Equals(MouseState other) => this == other;
        public override bool Equals(object obj) => obj is MouseState other && Equals(other);

        #endregion

        /// <summary>
        /// Returns the hash code of the <see cref="MouseState"/>.
        /// </summary>
        public override readonly int GetHashCode()
        {
            unchecked
            {
                int code = 17;
                code = code * 23 + X;
                code = code * 23 + Y;
                code = code * 23 + ScrollWheelValue;
                code = code * 23 + HorizontalScrollWheelValue;
                code = code * 23 + (int)LeftButton;
                code = code * 23 + (int)RightButton;
                code = code * 23 + (int)MiddleButton;
                code = code * 23 + (int)XButton1;
                code = code * 23 + (int)XButton2;
                return code;
            }
        }
    }
}

