
using System;

namespace MonoGame.Imaging
{
    internal unsafe delegate byte* ResampleDelegate(byte* a, byte* b, byte* c, int d, int e);

    internal unsafe struct Resample
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

    internal unsafe class JpgImage : IDisposable
    {
        public delegate void IDCT_BlockKernelDelegate(byte* output, int out_stride, short* data);
        public delegate void YCbCrToRGBKernelDelegate(byte* output, byte* y, byte* pcb, byte* pcr, int count, int step);

        public const int STBI__ZFAST_BITS = 9;

        public readonly ReadContext _readCtx;
        public readonly MarshalPointer _huff_dc;
        public readonly MarshalPointer _huff_ac;
        public readonly MarshalPointer[] _dequant;

        public readonly MarshalPointer[] _fastAc;

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
        public MarshalPointer _order;
        public int RestartInterval, ToDo;

        // kernels
        public IDCT_BlockKernelDelegate _IDCT_BlockKernel;
        public YCbCrToRGBKernelDelegate _YCbCrToRGBkernel;
        public ResampleDelegate _resampleRow_HV2_kernel;

        public JpgImage(ReadContext context)
        {
            this._readCtx = context;

            _huff_dc = Imaging.MAlloc(4 * sizeof(Huffman));
            _huff_ac = Imaging.MAlloc(4 * sizeof(Huffman));
            _order = Imaging.MAlloc(4 * sizeof(int));

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

            _fastAc = new MarshalPointer[4];
            for (var i = 0; i < _fastAc.Length; ++i)
            {
                _fastAc[i] = Imaging.MAlloc((1 << STBI__ZFAST_BITS) * sizeof(short));
            }

            _dequant = new MarshalPointer[4];
            for (var i = 0; i < _dequant.Length; ++i)
            {
                _dequant[i] = Imaging.MAlloc(64 * sizeof(ushort));
            }
        }
        
        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                for (int i = 0; i < _fastAc.Length; i++)
                    _fastAc[i].Dispose();

                for (int i = 0; i < _dequant.Length; i++)
                    _dequant[i].Dispose();

                _huff_dc.Dispose();
                _huff_ac.Dispose();
                _order.Dispose();

                _disposed = true;
            }
        }

        ~JpgImage()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
