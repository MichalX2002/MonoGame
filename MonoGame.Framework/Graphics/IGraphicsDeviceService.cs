// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    public interface IGraphicsDeviceService
    {
        GraphicsDevice GraphicsDevice { get; }
        
		event DataEvent<IGraphicsDeviceService> DeviceCreated;
        event DataEvent<IGraphicsDeviceService> DeviceDisposing;
        event DataEvent<IGraphicsDeviceService> DeviceReset;
        event DataEvent<IGraphicsDeviceService> DeviceResetting;
    }
}

