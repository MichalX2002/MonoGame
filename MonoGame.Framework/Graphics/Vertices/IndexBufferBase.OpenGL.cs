
namespace Microsoft.Xna.Framework.Graphics
{
    public abstract class IndexBufferBase : GraphicsResource
    {
        internal int _ibo;

        public int IndexCount { get; protected set; }
        public IndexElementSize IndexElementSize { get; protected set; }
    }
}
