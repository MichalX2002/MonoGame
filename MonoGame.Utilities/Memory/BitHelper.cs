
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Utilities.Memory
{
    public static class BitHelper
    {
        public static short ToInt16(this ReadOnlySpan<byte> value)
        {
            if (value.Length < sizeof(short))
                throw GetNotEnoughBytesException(nameof(value));
            return Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(value));
        }

        public static int ToInt32(this ReadOnlySpan<byte> value)
        {
            if (value.Length < sizeof(int))
                throw GetNotEnoughBytesException(nameof(value));
            return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(value));
        }

        private static ArgumentException GetNotEnoughBytesException(string argName)
        {
            return new ArgumentException("Not enough bytes.", argName);
        }
    }
}
