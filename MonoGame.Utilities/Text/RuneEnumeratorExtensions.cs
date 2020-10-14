using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Framework
{
    public static class RuneEnumeratorExtensions
    {
        public static RuneEnumerator GetRuneEnumerator(this ReadOnlySpan<char> text)
        {
            return text;
        }

        public static RuneEnumerator GetRuneEnumerator(this ReadOnlyMemory<char> text)
        {
            return text;
        }

        public static RuneEnumerator GetRuneEnumerator(this string text)
        {
            return text;
        }

        public static RuneEnumerator GetRuneEnumerator(this StringBuilder text)
        {
            return text;
        }

        public static RuneEnumerator GetRuneEnumerator(this IEnumerator<Rune> text)
        {
            return new RuneEnumerator(text);
        }

        public static RuneEnumerator GetRuneEnumerator(this IEnumerable<Rune> text)
        {
            return GetRuneEnumerator(text?.GetEnumerator());
        }
    }
}
