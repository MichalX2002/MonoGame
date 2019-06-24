namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Used to attach a vertex declaration to a vertex type so 
    /// it can be used in generic draw methods.
    /// </summary>
    public interface IVertexType
    {
        /// <summary>
        /// The vertex declaration describing this vertex type.
        /// </summary>
        VertexDeclaration VertexDeclaration { get; }
    }
}
