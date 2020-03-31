// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    public interface IGraphicsDeviceService
    {
        GraphicsDevice GraphicsDevice { get; }
        
        event DatalessEvent<IGraphicsDeviceService> DeviceCreated;
        event DatalessEvent<IGraphicsDeviceService> DeviceDisposing;
        event DatalessEvent<IGraphicsDeviceService> DeviceReset;
        event DatalessEvent<IGraphicsDeviceService> DeviceResetting;
    }
}

