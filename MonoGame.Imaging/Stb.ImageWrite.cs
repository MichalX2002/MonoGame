
using System.IO;

namespace MonoGame.Imaging
{
    unsafe partial class Imaging
    {
        public static WriteContext GetWriteContext(
            WriteCallback c, MemoryManager manager, Stream stream, SaveConfiguration config)
        {
            return new WriteContext(config.OnWrite, c, manager, stream, config.UseTgaRLE);
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
                            s.Write(s.Stream, &x, 1, s);
                            break;
                        }
                    case '2':
                        {
                            var x = v[vindex++];
                            var b = stackalloc byte[2];
                            b[0] = (byte)(x & 0xff);
                            b[1] = (byte)((x >> 8) & 0xff);
                            s.Write(s.Stream, b, 2, s);
                            break;
                        }
                    case '4':
                        {
                            var x = v[vindex++];
                            var b = stackalloc byte[4];
                            b[0] = (byte)(x & 0xff);
                            b[1] = (byte)((x >> 8) & 0xff);
                            b[2] = (byte)((x >> 16) & 0xff);
                            b[3] = (byte)((x >> 24) & 0xff);
                            s.Write(s.Stream, b, 4, s);
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

        public static int CallbackWriteBmp(
            in WriteContext s,
            int x,
            int y,
            int comp,
            void* data
            )
        {
            return WriteBmpCore(s, x, y, comp, data);
        }

        public static int CallbackWriteTga(in WriteContext s,
            int x,
            int y,
            int comp,
            void* data
            )
        {
            return WriteTgaCore(s, x, y, comp, data);
        }

        public static int CallbackWritePng(in WriteContext c,
            int x,
            int y,
            int comp,
            void* data,
            int stride_bytes)
        {
            int len;
            byte* png = MemoryWritePng(c.Manager, (byte*)data, stride_bytes, x, y, comp, &len);
            if (png == null)
                return 0;

            c.Write.Invoke(c.Stream, png, len, c);

            c.Manager.Free(png);
            return 1;
        }

        public static int CallbackWriteJpg(in WriteContext s, int jpgQuality,
            int x,
            int y,
            int comp,
            void* data)
        {
            return WriteJpgCore(s, x, y, comp, data, jpgQuality);
        }
    }
}
