﻿using System.Runtime.InteropServices;

namespace StbSharp
{
    internal static unsafe partial class StbImageResize
    {
        public delegate float KernelFunction(float x, float scale);
        public delegate float SupportFunction(float scale);

        public readonly struct FilterInfo
        {
            public readonly KernelFunction Kernel;
            public readonly SupportFunction Support;

            public FilterInfo(KernelFunction kernel, SupportFunction support)
            {
                Kernel = kernel;
                Support = support;
            }
        }

        public class ImageResizeInfo
        {
            public void* input_data;
            public int input_w;
            public int input_h;
            public int input_stride_bytes;
            public void* output_data;
            public int output_w;
            public int output_h;
            public int output_stride_bytes;
            public float s0;
            public float t0;
            public float s1;
            public float t1;
            public float horizontal_shift;
            public float vertical_shift;
            public float horizontal_scale;
            public float vertical_scale;
            public int channels;
            public int alpha_channel;
            public uint flags;
            public DataType type;
            public int horizontal_filter;
            public int vertical_filter;
            public int edge_horizontal;
            public int edge_vertical;
            public int colorspace;
            public stbir__contributors* horizontal_contributors;
            public float* horizontal_coefficients;
            public stbir__contributors* vertical_contributors;
            public float* vertical_coefficients;
            public int decode_buffer_pixels;
            public float* decode_buffer;
            public float* horizontal_buffer;
            public int horizontal_coefficient_width;
            public int vertical_coefficient_width;
            public int horizontal_filter_pixel_width;
            public int vertical_filter_pixel_width;
            public int horizontal_filter_pixel_margin;
            public int vertical_filter_pixel_margin;
            public int horizontal_num_contributors;
            public int vertical_num_contributors;
            public int ring_buffer_length_bytes;
            public int ring_buffer_num_entries;
            public int ring_buffer_first_scanline;
            public int ring_buffer_last_scanline;
            public int ring_buffer_begin_index;
            public float* ring_buffer;
            public float* encode_buffer;
            public int horizontal_contributors_size;
            public int horizontal_coefficients_size;
            public int vertical_contributors_size;
            public int vertical_coefficients_size;
            public int decode_buffer_size;
            public int horizontal_buffer_size;
            public int ring_buffer_size;
            public int encode_buffer_size;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct stbir__FP32
        {
            [FieldOffset(0)] public uint u;
            [FieldOffset(0)] public float f;
        }

        public static FilterInfo[] stbir__filter_info_table =
        {
            new FilterInfo(null, stbir__support_zero),
            new FilterInfo(stbir__filter_trapezoid, stbir__support_trapezoid),
            new FilterInfo(stbir__filter_triangle, stbir__support_one),
            new FilterInfo(stbir__filter_cubic, stbir__support_two),
            new FilterInfo(stbir__filter_catmullrom, stbir__support_two),
            new FilterInfo(stbir__filter_mitchell, stbir__support_two),
        };

        public static byte stbir__linear_to_srgb_uchar(float _in_)
        {
            var almostone = new stbir__FP32 { u = 0x3f7fffff };
            var minval = new stbir__FP32 { u = (127 - 13) << 23 };
            if (!(_in_ > minval.f)) _in_ = minval.f;
            if (_in_ > almostone.f) _in_ = almostone.f;

            var f = new stbir__FP32();
            f.f = _in_;
            uint tab = fp32_to_srgb8_tab4[(f.u - minval.u) >> 20];
            uint bias = (tab >> 16) << 9;
            uint scale = tab & 0xffff;
            uint t = (f.u >> 12) & 0xff;
            return (byte)((bias + scale * t) >> 16);
        }
    }
}