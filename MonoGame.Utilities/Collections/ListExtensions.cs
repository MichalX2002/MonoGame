using System.Collections.Generic;

namespace MonoGame.Framework.Collections
{
    public static class ListExtensions
    {
        public static ReadOnlyList<T> AsReadOnlyList<T>(this List<T> list)
        {
            return new ReadOnlyList<T>(list);
        }
    }
}
