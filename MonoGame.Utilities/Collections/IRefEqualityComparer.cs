
namespace MonoGame.Utilities.Collections
{
    public interface IRefEqualityComparer<T>
    {
        bool EqualsByRef(in T obj1, in T obj2);
        long GetLongHashCode(in T obj);
    }
}
