
namespace MonoGame.Imaging
{
    internal unsafe delegate byte* ResampleDelegate(byte* a, byte* b, byte* c, int d, int e);

    internal unsafe class Resample
    {
        public ResampleDelegate resampleDelegate;
        public byte* line0;
        public byte* line1;
        public int hs;
        public int vs;
        public int w_lores;
        public int ystep;
        public int ypos;
    }

    internal unsafe class JpgImage
    {
        public delegate void IDCT_BlockKernelDelegate(byte* output, int out_stride, short* data);
        public delegate void YCbCrToRGBKernelDelegate(byte* output, byte* y, byte* pcb, byte* pcr, int count, int step);

        public const int STBI__ZFAST_BITS = 9;

        public readonly ReadContext _context;
        public readonly MarshalPointer<Huffman> _huff_dc = new MarshalPointer<Huffman>(4);
        public readonly MarshalPointer<Huffman> _huff_ac = new MarshalPointer<Huffman>(4);
        public readonly MarshalPointer<ushort>[] _dequant;

        public readonly MarshalPointer<short>[] _fastAc;

        // sizes for components, interleaved MCUs
        public int img_h_max, img_v_max;
        public int img_mcu_x, img_mcu_y;
        public int img_mcu_w, img_mcu_h;

        // definition of jpeg image component
        public ImgComp[] JpgImgComp = new ImgComp[4];

        public uint _codeBuffer; // jpeg entropy-coded buffer
        public int _codeBits; // number of valid bits
        public byte _marker; // marker seen while filling entropy buffer
        public int _noMore; // flag if we saw a marker so must stop

        public int progressive;
        public int spec_start;
        public int spec_end;
        public int succ_high;
        public int succ_low;
        public int eob_run;
        public int jfif;
        public int app14_color_transform; // Adobe APP14 tag
        public int rgb;

        public int scan_n;
        public MarshalPointer<int> _order = new MarshalPointer<int>(4);
        public int RestartInterval, ToDo;

        // kernels
        public IDCT_BlockKernelDelegate _IDCT_BlockKernel;
        public YCbCrToRGBKernelDelegate _YCbCrToRGBkernel;
        public ResampleDelegate _resampleRow_HV2_kernel;

        public JpgImage(ReadContext context)
        {
            this._context = context;

            Huffman* huffAcP = (Huffman*)_huff_ac.Ptr;
            Huffman* huffDcP = (Huffman*)_huff_dc.Ptr;
            for (var i = 0; i < 4; ++i)
            {
                huffAcP[i] = new Huffman();
                huffDcP[i] = new Huffman();
            }

            for (var i = 0; i < JpgImgComp.Length; ++i)
            {
                JpgImgComp[i] = new ImgComp();
            }

            _fastAc = new MarshalPointer<short>[4];
            for (var i = 0; i < _fastAc.Length; ++i)
            {
                _fastAc[i] = new MarshalPointer<short>(1 << STBI__ZFAST_BITS);
            }

            _dequant = new MarshalPointer<ushort>[4];
            for (var i = 0; i < _dequant.Length; ++i)
            {
                _dequant[i] = new MarshalPointer<ushort>(64);
            }
        }
    }
}
