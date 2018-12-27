using System;

namespace MonoGame.Imaging
{
    public unsafe readonly struct MarshalPointer
    {
        private readonly bool _leaveOpen;
        public readonly int Size;
        public readonly byte* Ptr;

        public MarshalPointer(IntPtr ptr, int size) : this(ptr, false, size)
        {
        }

        public MarshalPointer(IntPtr ptr, bool leaveOpen, int size)
        {
            _leaveOpen = leaveOpen;
            Ptr = (byte*)ptr;
            Size = size;
        }

        public void Free()
        {
            if (!_leaveOpen && Ptr != null)
                Imaging.Free((IntPtr)Ptr);
        }
    }
}