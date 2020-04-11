using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Effect
{
    internal class MarshalHelper
    {
        public static T[] UnmarshalArray<T>(IntPtr ptr, int count)
        {
            var ret = new T[count];

            for (int i = 0; i < count; i++)
            {
                int offset = i * Unsafe.SizeOf<T>();
                var structPtr = new IntPtr(ptr.ToInt64() + offset);
                ret[i] = (T)Marshal.PtrToStructure(structPtr, typeof(T));
            }

            return ret;
        }
    }
}

