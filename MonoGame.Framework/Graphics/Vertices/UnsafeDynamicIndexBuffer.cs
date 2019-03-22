﻿
namespace Microsoft.Xna.Framework.Graphics
{
    public class UnsafeDynamicIndexBuffer : UnsafeIndexBuffer
    {
        public bool IsContentLost => false;

        public UnsafeDynamicIndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, BufferUsage bufferUsage) :
            base(graphicsDevice, indexElementSize, bufferUsage, true)
        {
        }
    }
}