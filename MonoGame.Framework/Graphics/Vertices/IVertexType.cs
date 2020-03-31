namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Used to attach a vertex declaration to a vertex type for use in generic draw methods.
    /// </summary>
    public interface IVertexType
    {
        /// <summary>
        /// The vertex declaration associated with the vertex type.
        /// </summary>
        VertexDeclaration VertexDeclaration { get; }
    }
}
