using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public class UnsafeDynamicVertexBuffer : UnsafeVertexBuffer
    {
        public bool IsContentLost { get { return false; } }

        public UnsafeDynamicVertexBuffer(
            GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, BufferUsage bufferUsage) :
            base(graphicsDevice, vertexDeclaration, bufferUsage, true)
        {
        }

        public UnsafeDynamicVertexBuffer(
            GraphicsDevice graphicsDevice, Type type, BufferUsage bufferUsage) :
            base(graphicsDevice, VertexDeclaration.FromType(type), bufferUsage, true)
        {
        }
    }
}
