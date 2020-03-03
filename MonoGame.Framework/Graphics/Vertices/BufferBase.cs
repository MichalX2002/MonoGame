// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    public abstract partial class BufferBase : GraphicsResource
    {
        internal bool _isDynamic;

        public int Capacity { get; }
        public int Count { get; protected set; }

        public BufferBase(GraphicsDevice graphicsDevice, int capacity) : base(graphicsDevice)
        {
            Capacity = capacity;
        }
    }
}
