// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    public abstract partial class BufferBase : GraphicsResource
    {
        public int Capacity { get; }
        public int Count { get; protected set; }

        public BufferUsage BufferUsage { get; private set; }
        public bool IsDynamic { get; }

        public BufferBase(
            GraphicsDevice graphicsDevice, 
            int capacity, 
            BufferUsage bufferUsage,
            bool isDynamic) : base(graphicsDevice)
        {
            Capacity = capacity;
            BufferUsage = bufferUsage;
            IsDynamic = isDynamic;
        }
    }
}
