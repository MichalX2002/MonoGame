using System;
using System.Buffers;

namespace MonoGame.Framework.Memory
{
    public class RecyclableArrayPool : ArrayPool<byte>
    {
        public static new RecyclableArrayPool Shared { get; } =
            new RecyclableArrayPool(RecyclableMemoryManager.Default);

        public RecyclableMemoryManager MemoryManager { get; }

        public RecyclableArrayPool(RecyclableMemoryManager memoryManager)
        {
            MemoryManager = memoryManager ?? throw new ArgumentNullException(nameof(memoryManager));
        }

        public override byte[] Rent(int minimumLength)
        {
            return MemoryManager.GetBuffer(minimumLength, null).Buffer;
        }

        public override void Return(byte[] array, bool clearArray = false)
        {
            if (clearArray)
                array.AsSpan().Clear();

            MemoryManager.ReturnBuffer(array, null);
        }
    }
}
