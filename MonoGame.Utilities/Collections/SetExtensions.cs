using System.Collections.Generic;

namespace MonoGame.Framework.Collections
{
    public static class SetExtensions
    {
        public static ReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
        {
            return new ReadOnlySet<T>(set);
        }
    }
}
