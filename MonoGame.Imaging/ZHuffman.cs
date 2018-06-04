using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ZHuffman
    {
        public fixed ushort fast[1 << 9];
        public fixed ushort firstcode[16];
        public fixed int maxcode[17];
        public fixed ushort firstsymbol[16];
        public fixed byte size[288];
        public fixed ushort value[288];
    }
}
