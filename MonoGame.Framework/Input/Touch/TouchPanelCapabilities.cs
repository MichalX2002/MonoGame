// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Input.Touch
{
    /// <summary>
    /// Allows retrieval of capability information from the touch panel device.
    /// </summary>
    public readonly partial struct TouchPanelCapabilities
    {
        /// <summary>
        /// Gets whether a touch device is available.
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Gets the maximum number of touch locations tracked by the touch panel device.
        /// </summary>
        public int MaximumTouchCount { get; }

        /// <summary>
        /// Gets whether the device has pressure sensitivity support.
        /// </summary>
        public bool HasPressure { get; }

        /// <summary>
        /// Constructs the <see cref="TouchPanelCapabilities"/>.
        /// </summary>
        public TouchPanelCapabilities(bool isConnected, int maximumTouchCount, bool hasPressure)
        {
            IsConnected = isConnected;
            MaximumTouchCount = maximumTouchCount;
            HasPressure = hasPressure;
        }
    }
}