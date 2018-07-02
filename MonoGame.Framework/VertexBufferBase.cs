
namespace Microsoft.Xna.Framework.Graphics
{
    public abstract class VertexBufferBase : GraphicsResource
    {
        internal abstract int VBO { get; }

        public int VertexCount { get; protected set; }
        public VertexDeclaration VertexDeclaration { get; protected set; }
    }
}
