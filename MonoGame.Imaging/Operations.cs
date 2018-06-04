using System.Collections.Concurrent;

namespace MonoGame.Imaging
{
    internal static unsafe class Operations
    {
        internal static ConcurrentDictionary<long, Pointer> _pointers = new ConcurrentDictionary<long, Pointer>();

        public static long AllocatedTotal
        {
            get { return Pointer.AllocatedTotal; }
        }

        public static void* MAlloc(long size)
        {
            var result = new PinnedArray<byte>(size);
            _pointers[(long) result.Ptr] = result;

            return result.Ptr;
        }

        public static void MemCopy(void* a, void* b, long size)
        {
            var ap = (byte*) a;
            var bp = (byte*) b;
            for (long i = 0; i < size; ++i)
            {
                *ap++ = *bp++;
            }
        }

        public static void MemMove(void* a, void* b, long size)
        {
            using (var temp = new PinnedArray<byte>(size))
            {
                MemCopy(temp.Ptr, b, size);
                MemCopy(a, temp.Ptr, size);
            }
        }

        public static void Free(void* a)
        {
            if (!_pointers.TryRemove((long)a, out Pointer pointer))
            {
                return;
            }

            pointer.Dispose();
        }

        public static void* ReAlloc(void* a, long newSize)
        {
            if (!_pointers.TryGetValue((long)a, out Pointer pointer))
            {
                // Allocate new
                return MAlloc(newSize);
            }

            if (newSize <= pointer.Size)
            {
                // Realloc not required
                return a;
            }

            var result = MAlloc(newSize);
            MemCopy(result, a, pointer.Size);

            _pointers.TryRemove((long) pointer.Ptr, out pointer);
            pointer.Dispose();

            return result;
        }

        public static int MemCmp(void* a, void* b, long size)
        {
            var result = 0;
            var ap = (byte*) a;
            var bp = (byte*) b;
            for (long i = 0; i < size; ++i)
            {
                if (*ap != *bp)
                {
                    result += 1;
                }
                ap++;
                bp++;
            }

            return result;
        }
    }
}
