
namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class VertexBufferBase : BufferBase
    {
        public int VertexCount { get; protected set; }
        public VertexDeclaration VertexDeclaration { get; protected set; }
    }
}
