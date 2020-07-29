using System;

namespace MonoGame.Framework.Collections
{
    public partial class LongEqualityComparer<T>
    {
        private sealed class LongIntPtrComparer : LongEqualityComparer<IntPtr>
        {
            public override long GetLongHashCode(IntPtr obj) => obj.ToInt64();
        }

        private sealed class LongUIntPtrComparer : LongEqualityComparer<UIntPtr>
        {
            public override long GetLongHashCode(UIntPtr obj) => unchecked((long)obj.ToUInt64());
        }
    }
}
