using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Collections
{
    public sealed class LongStringComparer : LongEqualityComparer<string?>
    {
        public override bool IsRandomized => true;

        public override long GetLongHashCode(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            var span = MemoryMarshal.AsBytes(value.AsSpan());
            var hash = MarvinHash64.ComputeHash(span, MarvinHash64.DefaultSeed);
            return MarvinHash64.CollapseHash64(hash);
        }
    }
}