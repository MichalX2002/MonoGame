using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GifLzw
    {
        public short Prefix;
        public byte First;
        public byte Suffix;
    }
}
