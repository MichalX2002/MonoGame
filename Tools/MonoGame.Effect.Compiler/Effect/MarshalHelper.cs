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

            for (int i = 0; i < ret.Length; i++)
            {
                var structPtr = ptr + i * Unsafe.SizeOf<T>();
                ret[i] = Marshal.PtrToStructure<T>(structPtr);
            }

            return ret;
        }
    }
}

