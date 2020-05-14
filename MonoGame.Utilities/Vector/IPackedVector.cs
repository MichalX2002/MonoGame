
namespace MonoGame.Framework.Vector
{
    // http://msdn.microsoft.com/en-us/library/bb197661.aspx
    public interface IPackedVector<TSelf, TPacked> : IVector<TSelf>
        where TSelf : IPackedVector<TSelf, TPacked>
    {
        /// <summary>
        /// Gets or sets the packed representation of this vector.
        /// </summary>
        TPacked PackedValue { get; set; }
    }
}
