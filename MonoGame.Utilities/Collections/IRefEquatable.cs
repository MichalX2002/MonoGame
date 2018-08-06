
namespace MonoGame.Utilities.Collections
{
    public interface IRefEquatable<T> : ILongHashable
    {
        bool EqualsByRef(in T item);
    }
}
