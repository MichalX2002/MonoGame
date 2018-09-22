using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ZBuffer
    {
        public byte* zbuffer;
        public byte* zbuffer_end;
        public int num_bits;
        public uint code_buffer;
        public sbyte* zout;
        public MarshalPointer zout_start;
        public sbyte* zout_end;
        public int z_expandable;
        public ZHuffman z_length;
        public ZHuffman z_distance;
    }
}
