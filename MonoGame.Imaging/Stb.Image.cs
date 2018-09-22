using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    internal unsafe partial class Imaging
    {
        public static int _verticallyFlipOnLoad;
        
        private static int Error(ErrorContext context, string error)
        {
            context.AddError(error);
            return 0;
        }

        /*
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void* MAlloc(MemoryManager manager, ulong size)
        {
            return manager.MAlloc((int) size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MemCopy(MemoryManager manager, void* a, void* b, ulong size)
        {
            manager.MemCopy(a, b, (long) size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MemMove(MemoryManager manager, void* a, void* b, ulong size)
        {
            manager.MemMove(a, b, (long) size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MemCmp(MemoryManager manager, void* a, void* b, ulong size)
        {
            return manager.MemCmp(a, b, (long) size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Free(MemoryManager manager, void* a)
        {
            manager.Free(a);
        }

        private static void* ReAlloc(MemoryManager manager, void* ptr, ulong newSize)
        {
            return manager.ReAlloc(ptr, (long)newSize);
        }
        */

        internal static unsafe MarshalPointer MAlloc(long size)
        {
            if (size == 0)
                return default;

            if (size > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(size));

            return new MarshalPointer(Marshal.AllocHGlobal((int)size), (int)size);
        }

        internal static void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        internal static void Free(void* ptr)
        {
            Free((IntPtr)ptr);
        }

        internal static void Free(MarshalPointer ptr)
        {
            ptr.Dispose();
        }

        internal static MarshalPointer ReAlloc(MarshalPointer p, long newSize)
        {
            if (p.Ptr == null)
                return MAlloc(newSize);

            if (newSize == 0)
            {
                Free(p);
                return default;
            }

            if (p.Size >= newSize)
                return p; // Realloc not required

            var newP = MAlloc(newSize);
            MemCopy(newP.Ptr, p.Ptr, p.Size);
            p.Dispose();
            return newP;
        }

        internal static void* ReAlloc(void* ptr, long newSize)
        { 
            if (ptr == null)
                return (void*)MAlloc(newSize).SourcePtr;
            return (void*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)newSize);
        }

        private static unsafe int MemCmp(void* a, void* b, long size)
        {
            int result = 0;
            byte* ap = (byte*)a;
            byte* bp = (byte*)b;

            for (long i = 0; i < size; ++i)
            {
                if (*ap != *bp)
                    result += 1;

                ap++;
                bp++;
            }

            return result;
        }

        private static unsafe void MemMove(void* a, void* b, long size)
        {
            using (var temp = MAlloc(size))
            {
                MemCopy(temp.Ptr, b, size);
                MemCopy(a, temp.Ptr, size);
            }
        }

        private static void MemSet(void* ptr, int value, ulong size)
        {
            byte* bptr = (byte*) ptr;
            var bval = (byte) value;

            for (ulong i = 0; i < size; ++i)
                *bptr++ = bval;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Rot32(uint x, int y)
        {
            return (x << y) | (x >> (32 - y));
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
        private static double DecomposeDouble(double number, int* exponent)
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
