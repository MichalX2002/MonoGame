using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MonoGame.Framework.Collections
{
    public interface ILongEqualityComparer<in T> : IEqualityComparer<T>
    {
        long GetLongHashCode([DisallowNull] T value);
    }
}
