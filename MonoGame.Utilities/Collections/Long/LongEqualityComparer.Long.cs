
namespace MonoGame.Framework.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private sealed class LongInt64Comparer : LongEqualityComparer<long>
        {
            public override long GetLongHashCode(long obj) => obj;
        }

        private sealed class LongUInt64Comparer : LongEqualityComparer<ulong>
        {
            public override long GetLongHashCode(ulong obj) => unchecked((long)obj);
        }
    }
}
