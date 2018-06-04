
namespace MonoGame.Imaging
{
    unsafe partial class Imaging
    {
        public struct WriteContext
        {
            public WriteCallback Callback;
            public void* Context;

            public WriteContext(WriteCallback callback, void* context)
            {
                Callback = callback;
                Context = context;
            }
        }

        public static int stbi_write_tga_with_rle = 1;

        public delegate int WriteCallback(void* context, void* data, int size);

        public static WriteContext GetWriteContext(WriteCallback c, void* context)
        {
            return new WriteContext(c, context);
        }

        public static void WriteFv(in WriteContext s, string fmt, params int[] v)
        {
            var vindex = 0;
            for (var i = 0; i < fmt.Length; ++i)
            {
                switch (fmt[i])
                {
                    case ' ':
                        break;

                    case '1':
                    {
                        var x = (byte)(v[vindex++] & 0xff);
                        s.Callback(s.Context, &x, 1);
                        break;
                    }
                    case '2':
                    {
                        var x = v[vindex++];
                        var b = stackalloc byte[2];
                        b[0] = (byte) (x & 0xff);
                        b[1] = (byte) ((x >> 8) & 0xff);
                        s.Callback(s.Context, b, 2);
                        break;
                    }
                    case '4':
                    {
                        var x = v[vindex++];
                        var b = stackalloc byte[4];
                        b[0] = (byte) (x & 0xff);
                        b[1] = (byte) ((x >> 8) & 0xff);
                        b[2] = (byte) ((x >> 16) & 0xff);
                        b[3] = (byte) ((x >> 24) & 0xff);
                        s.Callback(s.Context, b, 4);
                        break;
                    }
                }
            }
        }

        public static void WriteF(in WriteContext s, string fmt, params int[] v)
        {
            WriteFv(s, fmt, v);
        }

        public static int WriteOutFile(in WriteContext s, int rgb_dir, int vdir, int x, int y,
            int comp, int expand_mono, void* data, int alpha, int pad, string fmt, params int[] v)
        {
            if ((y < 0) || (x < 0))
            {
                return 0;
            }

            WriteFv(s, fmt, v);
            WritePixels(s, rgb_dir, vdir, x, y, comp, data, alpha, pad, expand_mono);
            return 1;
        }

        public static int CallbackWriteBmp(WriteCallback func,
            void* context,
            int x,
            int y,
            int comp,
            void* data
            )
        {
            WriteContext s = GetWriteContext(func, context);
            return WriteBmpCore(s, x, y, comp, data);
        }

        public static int CallbackWriteTga(WriteCallback func,
            void* context,
            int x,
            int y,
            int comp,
            void* data
            )
        {
            WriteContext s = GetWriteContext(func, context);
            return WriteTgaCore(s, x, y, comp, data);
        }

        public static int CallbackWritePng(WriteCallback func,
            void* context,
            int x,
            int y,
            int comp,
            void* data,
            int stride_bytes
            )
        {
            int len;
            var png = MemoryWritePng((byte*) (data), stride_bytes, x, y, comp, &len);
            if (png == null)
                return 0;
            func(context, png, len);
            Free(png);
            return 1;
        }

        public static int CallbackWriteJpg(WriteCallback func,
            void* context,
            int x,
            int y,
            int comp,
            void* data,
            int quality
            )
        {
            WriteContext s = GetWriteContext(func, context);
            return WriteJpgCore(s, x, y, comp, data, quality);
        }
    }
}
