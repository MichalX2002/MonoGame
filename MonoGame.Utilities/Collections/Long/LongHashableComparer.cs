using System.Diagnostics.CodeAnalysis;

namespace MonoGame.Framework.Collections
{
    internal class LongHashableComparer<T> : LongEqualityComparer<T>
        where T : ILongHashable
    {
        public override long GetLongHashCode([DisallowNull] T value) => value?.GetLongHashCode() ?? 0;
    }
}