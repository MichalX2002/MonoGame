
namespace MonoGame.Framework.Collections
{
    public partial class LongEqualityComparer<T>
    {
        /// <summary>
        /// Used on 64-bit machines.
        /// </summary>
        private class LongStringComparer64 : ILongEqualityComparer<string>
        {
            public bool Equals(string x, string y) => x.Equals(y);

            public int GetHashCode(string str) => str.GetHashCode();

            public unsafe long GetLongHashCode(string str)
            {
                fixed (char* src = str)
                {
                    int hash1 = 5381;
                    int hash2 = hash1;

                    int c;
                    char* s = src;
                    while ((c = s[0]) != 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ c;
                        c = s[1];
                        if (c == 0)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ c;
                        s += 2;
                    }

                    return hash1 + (hash2 * 1566083941);
                }
            }
        }

        /// <summary>
        /// Used on 32-bit machines.
        /// </summary>
        private class LongStringComparer32 : ILongEqualityComparer<string>
        {
            public bool Equals(string x, string y) => x.Equals(y);

            public int GetHashCode(string str) => str.GetHashCode();

            public unsafe long GetLongHashCode(string str)
            {
                fixed (char* src = str)
                {
                    int hash1 = (5381 << 16) + 5381;
                    int hash2 = hash1;

                    int* pint = (int*)src;
                    int len = str.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len -= 4;
                    }

                    if (len > 0)
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];

                    return hash1 + (hash2 * 1566083941);
                }
            }
        }
    }
}