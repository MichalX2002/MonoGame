using System;

namespace MonoGame.Imaging
{
    internal unsafe delegate byte* ResampleDelegate(byte* a, byte* b, byte* c, int d, int e);

    internal unsafe class Resample
    {
        public ResampleDelegate resample;
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
        public delegate void idct_BlockKernel(byte* output, int out_stride, short* data);

        public delegate void YCbCrToRGBKernel(
            byte* output, byte* y, byte* pcb, byte* pcr, int count, int step);

        public const int STBI__ZFAST_BITS = 9;

        public ReadContext s;
        public readonly PinnedArray<Huffman> huff_dc = new PinnedArray<Huffman>(4);
        public readonly PinnedArray<Huffman> huff_ac = new PinnedArray<Huffman>(4);
        public readonly PinnedArray<ushort>[] dequant;

        public readonly PinnedArray<short>[] fast_ac;

        // sizes for components, interleaved MCUs
        public int img_h_max, img_v_max;
        public int img_mcu_x, img_mcu_y;
        public int img_mcu_w, img_mcu_h;

        // definition of jpeg image component
        public ImgComp[] img_comp = new ImgComp[4];

        public uint code_buffer; // jpeg entropy-coded buffer
        public int code_bits; // number of valid bits
        public byte marker; // marker seen while filling entropy buffer
        public int nomore; // flag if we saw a marker so must stop

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
        public PinnedArray<int> order = new PinnedArray<int>(4);
        public int restart_interval, todo;

        // kernels
        public idct_BlockKernel idct_block_kernel;
        public YCbCrToRGBKernel YCbCr_to_RGB_kernel;
        public ResampleDelegate resample_row_hv_2_kernel;

        public JpgImage()
        {
            for (var i = 0; i < 4; ++i)
            {
                huff_ac[i] = new Huffman();
                huff_dc[i] = new Huffman();
            }

            for (var i = 0; i < img_comp.Length; ++i)
            {
                img_comp[i] = new ImgComp();
            }

            fast_ac = new PinnedArray<short>[4];
            for (var i = 0; i < fast_ac.Length; ++i)
            {
                fast_ac[i] = new PinnedArray<short>(1 << STBI__ZFAST_BITS);
            }

            dequant = new PinnedArray<ushort>[4];
            for (var i = 0; i < dequant.Length; ++i)
            {
                dequant[i] = new PinnedArray<ushort>(64);
            }
        }
    }
}
