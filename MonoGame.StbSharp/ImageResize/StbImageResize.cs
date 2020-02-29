using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public static unsafe partial class StbImageResize
    {
        public delegate float FilterKernelFunction(float x, float scale);
        public delegate float FilterSupportFunction(float scale);

        public class Filter
        {
            public static Filter Box { get; } = new Filter(FilterTrapezoid, SupportTrapezoid);
            public static Filter Triangle { get; } = new Filter(FilterTriangle, SupportOne);
            public static Filter Cubic { get; } = new Filter(FilterCubic, SupportTwo);
            public static Filter CatmullRom { get; } = new Filter(FilterCatmullRom, SupportTwo);
            public static Filter Mitchell { get; } = new Filter(FilterMitchell, SupportTwo);

            public FilterKernelFunction Kernel { get; }
            public FilterSupportFunction Support { get; }

            public Filter(FilterKernelFunction kernel, FilterSupportFunction support)
            {
                Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
                Support = support ?? throw new ArgumentNullException(nameof(support));
            }
        }

        public ref struct ResizeContext
        {
            public ReadOnlySpan<byte> input_data;
            public int input_w;
            public int input_h;
            public int input_stride_bytes;
            public Span<byte> output_data;
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
            public DataType datatype;
            public Filter horizontal_filter;
            public Filter vertical_filter;
            public WrapMode wrap_horizontal;
            public WrapMode wrap_vertical;
            public ColorSpace colorspace;
            public Span<Contributors> horizontal_contributors;
            public Span<float> horizontal_coefficients;
            public Span<Contributors> vertical_contributors;
            public Span<float> vertical_coefficients;
            public int decode_buffer_pixels;
            public Span<float> decode_buffer;
            public Span<float> horizontal_buffer;
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
            public Span<float> ring_buffer;
            public Span<float> encode_buffer;
            public int horizontal_contributors_size;
            public int horizontal_coefficients_size;
            public int vertical_contributors_size;
            public int vertical_coefficients_size;
            public int decode_buffer_size;
            public int horizontal_buffer_size;
            public int ring_buffer_size;
            public int encode_buffer_size;

            public int ring_buffer_length => ring_buffer_length_bytes / sizeof(float);
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FloatIntUnion
        {
            [FieldOffset(0)] public uint u;
            [FieldOffset(0)] public float f;
        }

        public static byte LinearToSrgbByte(float value)
        {
            var almost_one = new FloatIntUnion { u = 0x3f7fffff };
            var minval = new FloatIntUnion { u = (127 - 13) << 23 };

            if (!(value > minval.f))
                value = minval.f;
            if (value > almost_one.f)
                value = almost_one.f;

            var f = new FloatIntUnion();
            f.f = value;
            uint tab = float_to_srgb8_tab4[(int)((f.u - minval.u) >> 20)];
            uint bias = (tab >> 16) << 9;
            uint scale = tab & 0xffff;
            uint t = (f.u >> 12) & 0xff;
            return (byte)((bias + scale * t) >> 16);
        }
    }
}