// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Represents specific information about the state of the controller,
    /// including the current state of buttons and sticks.
    /// <para>
    /// This is implemented as a partial struct to allow for individual platforms
    /// to offer additional data without separate state queries to <see cref="GamePad"/>.
    /// </para>
    /// </summary>
    public readonly partial struct GamePadState : IEquatable<GamePadState>
    {
        /// <summary>
        /// The default initialized gamepad state.
        /// </summary>
        public static readonly GamePadState Default = new GamePadState();

        /// <summary>
        /// Gets a value indicating whether the controller is connected.
        /// </summary>
        public readonly bool IsConnected;

        /// <summary>
        /// Gets the packet number associated with this state.
        /// </summary>
        public readonly int PacketNumber;

        /// <summary>
        /// Gets a structure that identifies what buttons on the controller are pressed.
        /// </summary>
        public readonly GamePadButtons Buttons;

        /// <summary>
        /// Gets a structure that identifies what directions of the directional pad on the controller are pressed.
        /// </summary>
        public readonly GamePadDPad DPad;

        /// <summary>
        /// Gets a structure that indicates the position of the controller sticks (thumbsticks).
        /// </summary>
        public readonly GamePadThumbSticks ThumbSticks;

        /// <summary>
        /// Gets a structure that identifies the position of triggers on the controller.
        /// </summary>
        public readonly GamePadTriggers Triggers;

        internal GamePadState(
            in GamePadThumbSticks thumbSticks, in GamePadTriggers triggers,
            in GamePadButtons buttons, in GamePadDPad dPad, int packetNumber) : this()
        {
            ThumbSticks = thumbSticks;
            Triggers = triggers;
            Buttons = buttons;
            DPad = dPad;
            IsConnected = true;
            PacketNumber = packetNumber;

            PlatformConstruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadState"/>.
        /// </summary>
        /// <param name="thumbSticks">Initial thumbstick state.</param>
        /// <param name="triggers">Initial trigger state..</param>
        /// <param name="buttons">Initial button state.</param>
        /// <param name="dPad">Initial directional pad state.</param>
        public GamePadState(
            in GamePadThumbSticks thumbSticks, in GamePadTriggers triggers,
            in GamePadButtons buttons, in GamePadDPad dPad)
            : this(thumbSticks, triggers, buttons, dPad, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadState"/>
        /// using the specified stick, trigger, and button values.
        /// </summary>
        /// <param name="leftThumbStick">Left stick value. Each axis is clamped between −1.0 and 1.0.</param>
        /// <param name="rightThumbStick">Right stick value. Each axis is clamped between −1.0 and 1.0.</param>
        /// <param name="leftTrigger">Left trigger value. This value is clamped between 0.0 and 1.0.</param>
        /// <param name="rightTrigger">Right trigger value. This value is clamped between 0.0 and 1.0.</param>
        /// <param name="button">Button(s) to initialize as pressed.</param>
        public GamePadState(
            Vector2 leftThumbStick, Vector2 rightThumbStick, float leftTrigger, float rightTrigger, Buttons button)
            : this(
                new GamePadThumbSticks(leftThumbStick, rightThumbStick), 
                new GamePadTriggers(leftTrigger, rightTrigger),
                new GamePadButtons(button),
                new GamePadDPad(button))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadState"/> struct
        /// using the specified stick, trigger, and button values.
        /// </summary>
        /// <param name="leftThumbStick">Left stick value. Each axis is clamped between −1.0 and 1.0.</param>
        /// <param name="rightThumbStick">Right stick value. Each axis is clamped between −1.0 and 1.0.</param>
        /// <param name="leftTrigger">Left trigger value. This value is clamped between 0.0 and 1.0.</param>
        /// <param name="rightTrigger">Right trigger value. This value is clamped between 0.0 and 1.0.</param>
        /// <param name="buttons"> Array of Buttons to initialize as pressed.</param>
        public GamePadState(Vector2 leftThumbStick, Vector2 rightThumbStick, float leftTrigger, float rightTrigger, Buttons[] buttons) 
            : this(
                new GamePadThumbSticks(leftThumbStick, rightThumbStick), 
                new GamePadTriggers(leftTrigger, rightTrigger),
                new GamePadButtons(buttons), 
                new GamePadDPad(buttons))
        {
        }

        /// <summary>
        /// Define this method in platform partial classes to initialize default
        /// values for platform-specific fields.
        /// </summary>
        partial void PlatformConstruct();

        /// <summary>
        /// Gets the button mask along with 'virtual buttons' like LeftThumbstickLeft.
        /// </summary>
        private Buttons GetVirtualButtons ()
        {
            var result = Buttons._buttons;
            result |= ThumbSticks._virtualButtons;

            if (DPad.Down == ButtonState.Pressed)
                result |= Input.Buttons.DPadDown;
            if (DPad.Up == ButtonState.Pressed)
                result |= Input.Buttons.DPadUp;
            if (DPad.Left == ButtonState.Pressed)
                result |= Input.Buttons.DPadLeft;
            if (DPad.Right == ButtonState.Pressed)
                result |= Input.Buttons.DPadRight;

            return result;
        }

        /// <summary>
        /// Determines whether specified input device buttons are pressed in this GamePadState.
        /// </summary>
        /// <returns><c>true</c>, if button was pressed, <c>false</c> otherwise.</returns>
        /// <param name="button">Buttons to query. Specify a single button, or combine multiple buttons using a bitwise OR operation.</param>
        public bool IsButtonDown(Buttons button) => (GetVirtualButtons() & button) == button;

        /// <summary>
        /// Determines whether specified input device buttons are released (not pressed) in this GamePadState.
        /// </summary>
        /// <returns><c>true</c>, if button was released (not pressed), <c>false</c> otherwise.</returns>
        /// <param name="button">Buttons to query. Specify a single button, or combine multiple buttons using a bitwise OR operation.</param>
        public bool IsButtonUp(Buttons button) => (GetVirtualButtons() & button) != button;

        #region Equals

        public static bool operator ==(in GamePadState left, in GamePadState right)
        {
            return (left.IsConnected == right.IsConnected)
                && (left.PacketNumber == right.PacketNumber)
                && (left.Buttons == right.Buttons)
                && (left.DPad == right.DPad)
                && (left.ThumbSticks == right.ThumbSticks)
                && (left.Triggers == right.Triggers);
        }

        public static bool operator !=(in GamePadState left, in GamePadState right) => !(left == right);

        public bool Equals(GamePadState other) => this == other;
        public override bool Equals(object obj) => obj is GamePadState other && Equals(other);

        #endregion

        /// <summary>
        /// Returns the hash code of the <see cref="GamePadState"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int code = PacketNumber.GetHashCode();
                code = code * 23 + Buttons.GetHashCode();
                code = code * 23 + DPad.GetHashCode();
                code = code * 23 + ThumbSticks.GetHashCode();
                return code * 23 + Triggers.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="GamePadState"/>.
        /// </summary>
        public override string ToString()
        {
            if (!IsConnected)
                return "[GamePadState: IsConnected = 0]";

            return "[GamePadState: IsConnected=" + (IsConnected ? "1" : "0") +
                   ", PacketNumber=" + PacketNumber.ToString("00000") +
                   ", Buttons=" + Buttons +
                   ", DPad=" + DPad +
                   ", ThumbSticks=" + ThumbSticks +
                   ", Triggers=" + Triggers +
                   "]";
        }
    }
}
