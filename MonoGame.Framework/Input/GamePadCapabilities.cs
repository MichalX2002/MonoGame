// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Represents a controller's capabilities.
    /// </summary>
    public readonly struct GamePadCapabilities : IEquatable<GamePadCapabilities>
    {
        #region Fields

        /// <summary>
        /// Gets a value indicating whether the controller is connected.
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Gets the gamepad display name.
        /// This property is not available in XNA.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the unique identifier of the gamepad.
        /// This property is not available in XNA.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the button A.
        /// </summary>
        public bool HasAButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the button Back.
        /// </summary>
        public bool HasBackButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the button B.
        /// </summary>
        public bool HasBButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the directional pad down button.
        /// </summary>
        public bool HasDPadDownButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the directional pad left button.
        /// </summary>
        public bool HasDPadLeftButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the directional pad right button.
        /// </summary>
        public bool HasDPadRightButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the directional pad up button.
        /// </summary>
        public bool HasDPadUpButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the left shoulder button.
        /// </summary>
        public bool HasLeftShoulderButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the left stick button.
        /// </summary>
        public bool HasLeftStickButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the right shoulder button.
        /// </summary>
        public bool HasRightShoulderButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the right stick button.
        /// </summary>
        public bool HasRightStickButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the button Start.
        /// </summary>
        public bool HasStartButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the button X.
        /// </summary>
        public bool HasXButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the button Y.
        /// </summary>
        public bool HasYButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the guide button.
        /// </summary>
        public bool HasBigButton { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has X axis for the left stick (thumbstick) button.
        /// </summary>
        public bool HasLeftXThumbStick { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has Y axis for the left stick (thumbstick) button.
        /// </summary>
        public bool HasLeftYThumbStick { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has X axis for the right stick (thumbstick) button.
        /// </summary>
        public bool HasRightXThumbStick { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has Y axis for the right stick (thumbstick) button.
        /// </summary>
        public bool HasRightYThumbStick { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the left trigger button.
        /// </summary>
        public bool HasLeftTrigger { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the right trigger button.
        /// </summary>
        public bool HasRightTrigger { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the left vibration motor.
        /// </summary>
        public bool HasLeftVibrationMotor { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has the right vibration motor.
        /// </summary>
        public bool HasRightVibrationMotor { get; }

        /// <summary>
        /// Gets a value indicating whether the controller has a microphone.
        /// </summary>
        public bool HasVoiceSupport { get; }

        /// <summary>
        /// Gets the type of the controller.
        /// </summary>
        public GamePadType GamePadType { get; }

        #endregion

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

        #region Equals

        public static bool operator ==(in GamePadCapabilities left, in GamePadCapabilities right)
        {
            bool eq = true;
            eq &= left.DisplayName == right.DisplayName;
            eq &= left.Identifier == right.Identifier;
            eq &= left.IsConnected == right.IsConnected;
            eq &= left.HasAButton == right.HasAButton;
            eq &= left.HasBackButton == right.HasBackButton;
            eq &= left.HasBButton == right.HasBButton;
            eq &= left.HasDPadDownButton == right.HasDPadDownButton;
            eq &= left.HasDPadLeftButton == right.HasDPadLeftButton;
            eq &= left.HasDPadRightButton == right.HasDPadRightButton;
            eq &= left.HasDPadUpButton == right.HasDPadUpButton;
            eq &= left.HasLeftShoulderButton == right.HasLeftShoulderButton;
            eq &= left.HasLeftStickButton == right.HasLeftStickButton;
            eq &= left.HasRightShoulderButton == right.HasRightShoulderButton;
            eq &= left.HasRightStickButton == right.HasRightStickButton;
            eq &= left.HasStartButton == right.HasStartButton;
            eq &= left.HasXButton == right.HasXButton;
            eq &= left.HasYButton == right.HasYButton;
            eq &= left.HasBigButton == right.HasBigButton;
            eq &= left.HasLeftXThumbStick == right.HasLeftXThumbStick;
            eq &= left.HasLeftYThumbStick == right.HasLeftYThumbStick;
            eq &= left.HasRightXThumbStick == right.HasRightXThumbStick;
            eq &= left.HasRightYThumbStick == right.HasRightYThumbStick;
            eq &= left.HasLeftTrigger == right.HasLeftTrigger;
            eq &= left.HasRightTrigger == right.HasRightTrigger;
            eq &= left.HasLeftVibrationMotor == right.HasLeftVibrationMotor;
            eq &= left.HasRightVibrationMotor == right.HasRightVibrationMotor;
            eq &= left.HasVoiceSupport == right.HasVoiceSupport;
            eq &= left.GamePadType == right.GamePadType;
            return eq;
        }

        public static bool operator !=(in GamePadCapabilities a, in GamePadCapabilities b) => !(a == b);

        public bool Equals(GamePadCapabilities other) => this == other;
        public override bool Equals(object obj) => obj is GamePadCapabilities other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Returns the hash code of the <see cref="GamePadCapabilities"/>.
        /// </summary>
        public override int GetHashCode() => Identifier.GetHashCode();

        /// <summary>
        /// Returns a string that represents the current <see cref="GamePadCapabilities"/>.
        /// </summary>
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

        #endregion
    }
}
