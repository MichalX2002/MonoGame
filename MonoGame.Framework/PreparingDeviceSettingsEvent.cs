// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework
{
    /// <summary>
    /// The arguments to the <see cref="GraphicsDeviceManager.PreparingDeviceSettings"/> event.
    /// </summary>
    public readonly struct PreparingDeviceSettingsEvent
    {
        /// <summary>
        /// The default settings that will be used in device creation.
        /// </summary>
        public GraphicsDeviceInformation GraphicsDeviceInformation { get; }

        /// <summary>
        /// Create a new instance of the event.
        /// </summary>
        /// <param name="graphicsDeviceInformation">The default settings to be used in device creation.</param>
        public PreparingDeviceSettingsEvent(GraphicsDeviceInformation graphicsDeviceInformation)
        {
            GraphicsDeviceInformation = graphicsDeviceInformation;
        }
    }
}

