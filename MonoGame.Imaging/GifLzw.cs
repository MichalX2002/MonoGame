using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GifLzw
    {
        public short prefix;
        public byte first;
        public byte suffix;
    }
}
