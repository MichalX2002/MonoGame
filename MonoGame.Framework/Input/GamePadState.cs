// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;

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
        /// Gets a value indicating whether the controller is connected.
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Gets the packet number associated with this state.
        /// </summary>
        public int PacketNumber { get; }

        /// <summary>
        /// Gets a structure that identifies what buttons on the controller are pressed.
        /// </summary>
        public GamePadButtons Buttons { get; }

        /// <summary>
        /// Gets a structure that identifies what directions of the directional pad on the controller are pressed.
        /// </summary>
        public GamePadDPad DPad { get; }

        /// <summary>
        /// Gets a structure that indicates the position of the controller sticks (thumbsticks).
        /// </summary>
        public GamePadThumbSticks ThumbSticks { get; }

        /// <summary>
        /// Gets a structure that identifies the position of triggers on the controller.
        /// </summary>
        public GamePadTriggers Triggers { get; }

        internal GamePadState(
            bool isConnected,
            in GamePadThumbSticks thumbSticks,
            in GamePadTriggers triggers,
            in GamePadButtons buttons,
            in GamePadDPad dPad,
            int packetNumber) : this()
        {
            IsConnected = isConnected;
            ThumbSticks = thumbSticks;
            Triggers = triggers;
            Buttons = buttons;
            DPad = dPad;
            PacketNumber = packetNumber;

            PlatformConstruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadState"/>.
        /// </summary>
        /// <param name="isConnected">Whether any game pad is connected.</param>
        /// <param name="thumbSticks">Initial thumbstick state.</param>
        /// <param name="triggers">Initial trigger state..</param>
        /// <param name="buttons">Initial button state.</param>
        /// <param name="dPad">Initial directional pad state.</param>
        public GamePadState(
            bool isConnected,
            in GamePadThumbSticks thumbSticks,
            in GamePadTriggers triggers,
            in GamePadButtons buttons,
            in GamePadDPad dPad)
            : this(isConnected, thumbSticks, triggers, buttons, dPad, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadState"/>
        /// using the specified stick, trigger, and button values.
        /// </summary>
        /// <param name="isConnected">Whether any game pad is connected.</param>
        /// <param name="leftThumbStick">Left stick value. Each axis is clamped between −1.0 and 1.0.</param>
        /// <param name="rightThumbStick">Right stick value. Each axis is clamped between −1.0 and 1.0.</param>
        /// <param name="leftTrigger">Left trigger value. This value is clamped between 0.0 and 1.0.</param>
        /// <param name="rightTrigger">Right trigger value. This value is clamped between 0.0 and 1.0.</param>
        /// <param name="button">Button(s) to initialize as pressed.</param>
        public GamePadState(
            bool isConnected,
            Vector2 leftThumbStick,
            Vector2 rightThumbStick,
            float leftTrigger,
            float rightTrigger,
            Buttons button) : this(
                isConnected,
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
        /// <param name="isConnected">Whether any game pad is connected.</param>
        /// <param name="leftThumbStick">Left stick value. Each axis is clamped between −1.0 and 1.0.</param>
        /// <param name="rightThumbStick">Right stick value. Each axis is clamped between −1.0 and 1.0.</param>
        /// <param name="leftTrigger">Left trigger value. This value is clamped between 0.0 and 1.0.</param>
        /// <param name="rightTrigger">Right trigger value. This value is clamped between 0.0 and 1.0.</param>
        /// <param name="buttons">Span of Buttons to initialize as pressed.</param>
        public GamePadState(
            bool isConnected,
            Vector2 leftThumbStick,
            Vector2 rightThumbStick,
            float leftTrigger,
            float rightTrigger,
            ReadOnlySpan<Buttons> buttons) : this(
                isConnected,
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
        private Buttons GetVirtualButtons()
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
        /// <returns><see langword="true"/>, if button was pressed, <see langword="false"/> otherwise.</returns>
        /// <param name="button">Buttons to query. Specify a single button, or combine multiple buttons using a bitwise OR operation.</param>
        public bool IsButtonDown(Buttons button) => (GetVirtualButtons() & button) == button;

        /// <summary>
        /// Determines whether specified input device buttons are released (not pressed) in this GamePadState.
        /// </summary>
        /// <returns><see langword="true"/>, if button was released (not pressed), <see langword="false"/> otherwise.</returns>
        /// <param name="button">Buttons to query. Specify a single button, or combine multiple buttons using a bitwise OR operation.</param>
        public bool IsButtonUp(Buttons button) => (GetVirtualButtons() & button) != button;

        #region Equals

        public bool Equals(GamePadState other) => this == other;

        public override bool Equals(object? obj) => obj is GamePadState other && Equals(other);

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

        #endregion

        /// <summary>
        /// Returns the hash code of the <see cref="GamePadState"/>.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(PacketNumber, Buttons, DPad, ThumbSticks, Triggers);

        /// <summary>
        /// Returns a string that represents the current <see cref="GamePadState"/>.
        /// </summary>
        public override string ToString()
        {
            if (!IsConnected)
                return "[GamePadState: IsConnected=false]";

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
