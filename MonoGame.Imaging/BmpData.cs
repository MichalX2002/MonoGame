using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BmpInfo
    {
        public int bpp;
        public int offset;
        public int hsz;
        public uint mr;
        public uint mg;
        public uint mb;
        public uint ma;
        public uint all_a;
    }
}
