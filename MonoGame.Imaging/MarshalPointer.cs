using System;

namespace MonoGame.Imaging
{
    internal unsafe struct MarshalPointer : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public readonly IntPtr SourcePtr;
        public readonly int Size;
        public readonly byte* Ptr;
        
        public MarshalPointer(IntPtr ptr, int size)
        {
            SourcePtr = ptr;
            Ptr = (byte*)SourcePtr;
            Size = size;
            IsDisposed = false;
        }
        
        public void Dispose()
        {
            if (IsDisposed == false)
            {
                Imaging.Free(SourcePtr);
                IsDisposed = true;
            }
        }
    }
}
