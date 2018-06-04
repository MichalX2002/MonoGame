using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Huffman
    {
        public fixed byte fast[1 << 9];
        public fixed ushort code[256];
        public fixed byte values[256];
        public fixed byte size[257];
        public fixed uint maxcode[18];
        public fixed int delta[17];
    }
}
