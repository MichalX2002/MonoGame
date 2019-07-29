namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Helper class which ensures we only lookup a vertex 
    /// declaration for a particular type once.
    /// </summary>
    /// <typeparam name="T">An unmanaged vertex structure which implements <see cref="IVertexType"/>.</typeparam>
    internal class VertexDeclarationCache<T> where T : unmanaged, IVertexType
    {
        private static VertexDeclaration _cached;

        public static VertexDeclaration VertexDeclaration
        {
            get
            {
                if (_cached == null)
                    _cached = VertexDeclaration.FromType(typeof(T));
                return _cached;
            }
        }
    }
}
