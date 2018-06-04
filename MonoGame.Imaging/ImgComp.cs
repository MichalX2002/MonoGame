using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ImgComp
    {
        public int id;
        public int h, v;
        public int tq;
        public int hd, ha;
        public int dc_pred;

        public int x, y, w2, h2;
        public byte* data;
        public void* raw_data;
        public void* raw_coeff;
        public byte* linebuf;
        public short* coeff; // progressive only
        public int coeff_w, coeff_h; // number of 8x8 coefficient blocks
    }
}
