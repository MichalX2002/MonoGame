using System.Collections.Generic;

namespace MonoGame.Utilities.Collections
{
    public interface ILongEqualityComparer<in T> : IEqualityComparer<T>
    {
        long GetLongHashCode(T value);
    }
}
