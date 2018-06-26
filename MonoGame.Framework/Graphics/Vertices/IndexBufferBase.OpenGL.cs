
namespace Microsoft.Xna.Framework.Graphics
{
    public abstract class IndexBufferBase : GraphicsResource
    {
        internal abstract int IBO { get; }

        public int IndexCount { get; protected set; }
        public IndexElementSize IndexElementSize { get; protected set; }
    }
}
