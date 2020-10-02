using System;
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
    }
}
