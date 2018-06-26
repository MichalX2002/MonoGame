
namespace Microsoft.Xna.Framework.Graphics.Vertices
{
    public class UnsafeDynamicIndexBuffer : UnsafeIndexBuffer
    {
        public bool IsContentLost { get { return false; } }

        public UnsafeDynamicIndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, BufferUsage bufferUsage) :
            base(graphicsDevice, indexElementSize, bufferUsage, true)
        {
        }
    }
}
