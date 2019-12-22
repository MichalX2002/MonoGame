using System.Collections.Generic;

namespace MonoGame.Framework.Collections
{
    public interface ILongEqualityComparer<in T> : IEqualityComparer<T>
    {
        long GetLongHashCode(T value);
    }
}
