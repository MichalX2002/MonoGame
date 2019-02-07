using System.Collections.Generic;

namespace MonoGame.Utilities.Collections
{
    public interface ILongEqualityComparer<T> : IEqualityComparer<T>
    {
        long GetLongHashCode(T obj);
    }
}
