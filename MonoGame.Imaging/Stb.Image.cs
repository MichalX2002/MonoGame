using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Imaging
{
    internal unsafe partial class Imaging
    {
        public static string LastError;

        public static string stbi__g_failure_reason;
        public static int stbi__vertically_flip_on_load;

        private static int Error(string str)
        {
            LastError = str;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void* MAlloc(ulong size)
        {
            return Operations.MAlloc((int) size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MemCopy(void* a, void* b, ulong size)
        {
            Operations.MemCopy(a, b, (long) size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MemMove(void* a, void* b, ulong size)
        {
            Operations.MemMove(a, b, (long) size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MemCmp(void* a, void* b, ulong size)
        {
            return Operations.MemCmp(a, b, (long) size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Free(void* a)
        {
            Operations.Free(a);
        }

        private static void MemSet(void* ptr, int value, ulong size)
        {
            byte* bptr = (byte*) ptr;
            var bval = (byte) value;
            for (ulong i = 0; i < size; ++i)
            {
                *bptr++ = bval;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint _lrotl(uint x, int y)
        {
            return (x << y) | (x >> (32 - y));
        }

        private static void* ReAlloc(void* ptr, ulong newSize)
        {
            return Operations.ReAlloc(ptr, (long)newSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Abs(int v)
        {
            return (v + (v >> 31)) ^ (v >> 31);
        }

        public static void GifParseColortable(ReadContext s, byte* pal, int num_entries, int transp)
        {
            int i;
            for (i = 0; (i) < (num_entries); ++i)
            {
                pal[i*4 + 2] = GetByte(s);
                pal[i*4 + 1] = GetByte(s);
                pal[i*4] = GetByte(s);
                pal[i*4 + 3] = (byte) (transp == i ? 0 : 255);
            }
        }

        public const long DBL_EXP_MASK = 0x7ff0000000000000L;
        public const int DBL_MANT_BITS = 52;
        public const long DBL_SGN_MASK = -1 - 0x7fffffffffffffffL;
        public const long DBL_MANT_MASK = 0x000fffffffffffffL;
        public const long DBL_EXP_CLR_MASK = DBL_SGN_MASK | DBL_MANT_MASK;

        /// <summary>
        /// This code had been borrowed from here: https://github.com/MachineCognitis/C.math.NET
        /// </summary>
        /// <param name="number"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        private static double FRExp(double number, int* exponent)
        {
            var bits = BitConverter.DoubleToInt64Bits(number);
            var exp = (int) ((bits & DBL_EXP_MASK) >> DBL_MANT_BITS);
            *exponent = 0;

            if (exp == 0x7ff || number == 0D)
                number += number;
            else
            {
                // Not zero and finite.
                *exponent = exp - 1022;
                if (exp == 0)
                {
                    // Subnormal, scale number so that it is in [1, 2).
                    number *= BitConverter.Int64BitsToDouble(0x4350000000000000L); // 2^54
                    bits = BitConverter.DoubleToInt64Bits(number);
                    exp = (int) ((bits & DBL_EXP_MASK) >> DBL_MANT_BITS);
                    *exponent = exp - 1022 - 54;
                }
                // Set exponent to -1 so that number is in [0.5, 1).
                number = BitConverter.Int64BitsToDouble((bits & DBL_EXP_CLR_MASK) | 0x3fe0000000000000L);
            }

            return number;
        }
    }
}
