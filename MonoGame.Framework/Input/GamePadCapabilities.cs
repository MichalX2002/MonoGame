// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Represents a controller's capabilities.
    /// </summary>
    public readonly struct GamePadCapabilities
    {
        public static readonly GamePadCapabilities None = new GamePadCapabilities();

        /// <summary>
        /// Gets a value indicating if the controller is connected.
        /// </summary>
        /// <value><c>true</c> if it is connected; otherwise, <c>false</c>.</value>
        public readonly bool IsConnected;

        /// <summary>
        /// Gets the gamepad display name.
        /// 
        /// This property is not available in XNA.
        /// </summary>
        /// <value>String representing the display name of the gamepad.</value>
        public readonly string DisplayName;

        /// <summary>
        /// Gets the unique identifier of the gamepad.
        /// 
        /// This property is not available in XNA.
        /// </summary>
        /// <value>String representing the unique identifier of the gamepad.</value>
        public readonly string Identifier;

        /// <summary>
        /// Gets a value indicating whether the controller has the button A.
        /// </summary>
        /// <value><c>true</c> if it has the button A; otherwise, <c>false</c>.</value>
        public readonly bool HasAButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the button Back.
        /// </summary>
        /// <value><c>true</c> if it has the button Back; otherwise, <c>false</c>.</value>
        public readonly bool HasBackButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the button B.
        /// </summary>
        /// <value><c>true</c> if it has the button B; otherwise, <c>false</c>.</value>
        public readonly bool HasBButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the directional pad down button.
        /// </summary>
        /// <value><c>true</c> if it has the directional pad down button; otherwise, <c>false</c>.</value>
        public readonly bool HasDPadDownButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the directional pad left button.
        /// </summary>
        /// <value><c>true</c> if it has the directional pad left button; otherwise, <c>false</c>.</value>
        public readonly bool HasDPadLeftButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the directional pad right button.
        /// </summary>
        /// <value><c>true</c> if it has the directional pad right button; otherwise, <c>false</c>.</value>
        public readonly bool HasDPadRightButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the directional pad up button.
        /// </summary>
        /// <value><c>true</c> if it has the directional pad up button; otherwise, <c>false</c>.</value>
        public readonly bool HasDPadUpButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the left shoulder button.
        /// </summary>
        /// <value><c>true</c> if it has the left shoulder button; otherwise, <c>false</c>.</value>
        public readonly bool HasLeftShoulderButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the left stick button.
        /// </summary>
        /// <value><c>true</c> if it has the left stick button; otherwise, <c>false</c>.</value>
        public readonly bool HasLeftStickButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the right shoulder button.
        /// </summary>
        /// <value><c>true</c> if it has the right shoulder button; otherwise, <c>false</c>.</value>
        public readonly bool HasRightShoulderButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the right stick button.
        /// </summary>
        /// <value><c>true</c> if it has the right stick button; otherwise, <c>false</c>.</value>
        public readonly bool HasRightStickButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the button Start.
        /// </summary>
        /// <value><c>true</c> if it has the button Start; otherwise, <c>false</c>.</value>
        public readonly bool HasStartButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the button X.
        /// </summary>
        /// <value><c>true</c> if it has the button X; otherwise, <c>false</c>.</value>
        public readonly bool HasXButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the button Y.
        /// </summary>
        /// <value><c>true</c> if it has the button Y; otherwise, <c>false</c>.</value>
        public readonly bool HasYButton;

        /// <summary>
        /// Gets a value indicating whether the controller has the guide button.
        /// </summary>
        /// <value><c>true</c> if it has the guide button; otherwise, <c>false</c>.</value>
        public readonly bool HasBigButton;

        /// <summary>
        /// Gets a value indicating whether the controller has X axis for the left stick (thumbstick) button.
        /// </summary>
        /// <value><c>true</c> if it has X axis for the left stick (thumbstick) button; otherwise, <c>false</c>.</value>
        public readonly bool HasLeftXThumbStick;

        /// <summary>
        /// Gets a value indicating whether the controller has Y axis for the left stick (thumbstick) button.
        /// </summary>
        /// <value><c>true</c> if it has Y axis for the left stick (thumbstick) button; otherwise, <c>false</c>.</value>
        public readonly bool HasLeftYThumbStick;

        /// <summary>
        /// Gets a value indicating whether the controller has X axis for the right stick (thumbstick) button.
        /// </summary>
        /// <value><c>true</c> if it has X axis for the right stick (thumbstick) button; otherwise, <c>false</c>.</value>
        public readonly bool HasRightXThumbStick;

        /// <summary>
        /// Gets a value indicating whether the controller has Y axis for the right stick (thumbstick) button.
        /// </summary>
        /// <value><c>true</c> if it has Y axis for the right stick (thumbstick) button; otherwise, <c>false</c>.</value>
        public readonly bool HasRightYThumbStick;

        /// <summary>
        /// Gets a value indicating whether the controller has the left trigger button.
        /// </summary>
        /// <value><c>true</c> if it has the left trigger button; otherwise, <c>false</c>.</value>
        public readonly bool HasLeftTrigger;

        /// <summary>
        /// Gets a value indicating whether the controller has the right trigger button.
        /// </summary>
        /// <value><c>true</c> if it has the right trigger button; otherwise, <c>false</c>.</value>
        public readonly bool HasRightTrigger;

        /// <summary>
        /// Gets a value indicating whether the controller has the left vibration motor.
        /// </summary>
        /// <value><c>true</c> if it has the left vibration motor; otherwise, <c>false</c>.</value>
        public readonly bool HasLeftVibrationMotor;

        /// <summary>
        /// Gets a value indicating whether the controller has the right vibration motor.
        /// </summary>
        /// <value><c>true</c> if it has the right vibration motor; otherwise, <c>false</c>.</value>
        public readonly bool HasRightVibrationMotor;

        /// <summary>
        /// Gets a value indicating whether the controller has a microphone.
        /// </summary>
        /// <value><c>true</c> if it has a microphone; otherwise, <c>false</c>.</value>
        public readonly bool HasVoiceSupport;

        /// <summary>
        /// Gets the type of the controller.
        /// </summary>
        /// <value>A <see cref="GamePadType"/> representing the controller type..</value>
        public readonly GamePadType GamePadType;

        public GamePadCapabilities(
            bool isConnected, string displayName, string identifier, 
            bool hasAButton, bool hasBButton, bool hasBackButton,
            bool hasDPadDownButton, bool hasDPadLeftButton, bool hasDPadRightButton, bool hasDPadUpButton,
            bool hasLeftShoulderButton, bool hasLeftStickButton, 
            bool hasRightShoulderButton, bool hasRightStickButton, 
            bool hasStartButton, bool hasXButton, bool hasYButton, bool hasBigButton, 
            bool hasLeftXThumbStick, bool hasLeftYThumbStick, bool hasRightXThumbStick, bool hasRightYThumbStick,
            bool hasLeftTrigger, bool hasRightTrigger, bool hasLeftVibrationMotor, bool hasRightVibrationMotor,
            bool hasVoiceSupport, GamePadType gamePadType)
        {
            IsConnected = isConnected;
            DisplayName = displayName;
            Identifier = identifier;
            HasAButton = hasAButton;
            HasBButton = hasBButton;
            HasBackButton = hasBackButton;
            HasDPadDownButton = hasDPadDownButton;
            HasDPadLeftButton = hasDPadLeftButton;
            HasDPadRightButton = hasDPadRightButton;
            HasDPadUpButton = hasDPadUpButton;
            HasLeftShoulderButton = hasLeftShoulderButton;
            HasLeftStickButton = hasLeftStickButton;
            HasRightShoulderButton = hasRightShoulderButton;
            HasRightStickButton = hasRightStickButton;
            HasStartButton = hasStartButton;
            HasXButton = hasXButton;
            HasYButton = hasYButton;
            HasBigButton = hasBigButton;
            HasLeftXThumbStick = hasLeftXThumbStick;
            HasLeftYThumbStick = hasLeftYThumbStick;
            HasRightXThumbStick = hasRightXThumbStick;
            HasRightYThumbStick = hasRightYThumbStick;
            HasLeftTrigger = hasLeftTrigger;
            HasRightTrigger = hasRightTrigger;
            HasLeftVibrationMotor = hasLeftVibrationMotor;
            HasRightVibrationMotor = hasRightVibrationMotor;
            HasVoiceSupport = hasVoiceSupport;
            GamePadType = gamePadType;
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="GamePadCapabilities"/>
        /// is equal to another specified <see cref="GamePadCapabilities"/>.
        /// </summary>
        /// <param name="left">The first <see cref="GamePadCapabilities"/> to compare.</param>
        /// <param name="right">The second <see cref="GamePadCapabilities"/> to compare.</param>
        /// <returns><c>true</c> if <c>left</c> and <c>right</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(in GamePadCapabilities left, in GamePadCapabilities right)
        {
            bool eq = true;
            eq &= (left.DisplayName == right.DisplayName);
            eq &= (left.Identifier == right.Identifier);
            eq &= (left.IsConnected == right.IsConnected);
            eq &= (left.HasAButton == right.HasAButton);
            eq &= (left.HasBackButton == right.HasBackButton);
            eq &= (left.HasBButton == right.HasBButton);
            eq &= (left.HasDPadDownButton == right.HasDPadDownButton);
            eq &= (left.HasDPadLeftButton == right.HasDPadLeftButton);
            eq &= (left.HasDPadRightButton == right.HasDPadRightButton);
            eq &= (left.HasDPadUpButton == right.HasDPadUpButton);
            eq &= (left.HasLeftShoulderButton == right.HasLeftShoulderButton);
            eq &= (left.HasLeftStickButton == right.HasLeftStickButton);
            eq &= (left.HasRightShoulderButton == right.HasRightShoulderButton);
            eq &= (left.HasRightStickButton == right.HasRightStickButton);
            eq &= (left.HasStartButton == right.HasStartButton);
            eq &= (left.HasXButton == right.HasXButton);
            eq &= (left.HasYButton == right.HasYButton);
            eq &= (left.HasBigButton == right.HasBigButton);
            eq &= (left.HasLeftXThumbStick == right.HasLeftXThumbStick);
            eq &= (left.HasLeftYThumbStick == right.HasLeftYThumbStick);
            eq &= (left.HasRightXThumbStick == right.HasRightXThumbStick);
            eq &= (left.HasRightYThumbStick == right.HasRightYThumbStick);
            eq &= (left.HasLeftTrigger == right.HasLeftTrigger);
            eq &= (left.HasRightTrigger == right.HasRightTrigger);
            eq &= (left.HasLeftVibrationMotor == right.HasLeftVibrationMotor);
            eq &= (left.HasRightVibrationMotor == right.HasRightVibrationMotor);
            eq &= (left.HasVoiceSupport == right.HasVoiceSupport);
            eq &= (left.GamePadType == right.GamePadType);
            return eq;
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="GamePadCapabilities"/>
        /// is not equal to another specified <see cref="GamePadCapabilities"/>.
        /// </summary>
        /// <param name="left">The first <see cref="GamePadCapabilities"/> to compare.</param>
        /// <param name="right">The second <see cref="GamePadCapabilities"/> to compare.</param>
        /// <returns><c>true</c> if <c>left</c> and <c>right</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(in GamePadCapabilities left, in GamePadCapabilities right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="GamePadCapabilities"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="GamePadCapabilities"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="GamePadCapabilities"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return (obj is GamePadCapabilities other) && (this == other);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="GamePadCapabilities"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="GamePadCapabilities"/>.
        /// </summary>
        /// <returns>A string that represents the current <see cref="GamePadCapabilities"/>.</returns>
        public override string ToString()
        {
            return "[GamePadCapabilities: IsConnected=" + IsConnected +
                ", DisplayName=" + DisplayName +
                ", Identifier=" + Identifier +
                ", HasAButton=" + HasAButton +
                ", HasBackButton=" + HasBackButton +
                ", HasBButton=" + HasBButton +
                ", HasDPadDownButton=" + HasDPadDownButton +
                ", HasDPadLeftButton=" + HasDPadLeftButton +
                ", HasDPadRightButton=" + HasDPadRightButton +
                ", HasDPadUpButton=" + HasDPadUpButton +
                ", HasLeftShoulderButton=" + HasLeftShoulderButton +
                ", HasLeftStickButton=" + HasLeftStickButton +
                ", HasRightShoulderButton=" + HasRightShoulderButton +
                ", HasRightStickButton=" + HasRightStickButton +
                ", HasStartButton=" + HasStartButton +
                ", HasXButton=" + HasXButton +
                ", HasYButton=" + HasYButton +
                ", HasBigButton=" + HasBigButton +
                ", HasLeftXThumbStick=" + HasLeftXThumbStick +
                ", HasLeftYThumbStick=" + HasLeftYThumbStick +
                ", HasRightXThumbStick=" + HasRightXThumbStick +
                ", HasRightYThumbStick=" + HasRightYThumbStick +
                ", HasLeftTrigger=" + HasLeftTrigger +
                ", HasRightTrigger=" + HasRightTrigger +
                ", HasLeftVibrationMotor=" + HasLeftVibrationMotor +
                ", HasRightVibrationMotor=" + HasRightVibrationMotor +
                ", HasVoiceSupport=" + HasVoiceSupport +
                ", GamePadType=" + GamePadType +
                "]";
        }
    }
}
