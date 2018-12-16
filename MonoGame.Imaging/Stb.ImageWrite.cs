
using System.IO;

namespace MonoGame.Imaging
{
    internal unsafe partial class Imaging
    {
        public static WriteContext GetWriteContext(
            WriteCallback c,Stream stream, byte[] buffer, SaveConfiguration config)
        {
            return new WriteContext(c, stream, buffer, config.UseTgaRLE);
        }

        public static void WriteFv(WriteContext s, string fmt, params int[] v)
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
                            byte x = (byte)(v[vindex++] & 0xff);
                            WriteChar(s, x);
                            break;
                        }
                    case '2':
                        {
                            var x = v[vindex++];
                            var b = stackalloc byte[2];
                            b[0] = (byte)(x & 0xff);
                            b[1] = (byte)((x >> 8) & 0xff);
                            s.Write(s.Stream, b, s.Buffer, 2);
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
                            s.Write(s.Stream, b, s.Buffer, 4);
                            break;
                        }
                }
            }
        }

        public static void WriteF(WriteContext s, string fmt, params int[] v)
        {
            WriteFv(s, fmt, v);
        }

        public static int WriteFile(WriteContext s, int rgb_dir, int vdir, int x, int y,
            int comp, int expand_mono, void* data, int alpha, int pad, string fmt, params int[] v)
        {
            if ((y < 0) || (x < 0))
                return 0;

            WriteFv(s, fmt, v);
            WritePixels(s, rgb_dir, vdir, x, y, comp, data, alpha, pad, expand_mono);
            return 1;
        }

        public static int CallbackWriteBmp(
            WriteContext s,
            int x,
            int y,
            int comp,
            void* data
            )
        {
            return WriteBmpCore(s, x, y, comp, data);
        }

        public static int CallbackWriteTga(
            WriteContext s,
            int x,
            int y,
            int comp,
            void* data
            )
        {
            return WriteTgaCore(s, x, y, comp, data);
        }

        public static int CallbackWritePng(
            WriteContext s,
            int x,
            int y,
            int comp,
            byte* data,
            int stride_bytes)
        {
            int len;
            byte* png = MemoryWritePng(data, stride_bytes, x, y, comp, &len);
            if (png == null)
                return 0;

            s.Write(s.Stream, png, s.Buffer, len);

            Free(png);
            return 1;
        }

        public static int CallbackWriteJpg(
            WriteContext s,
            int jpgQuality,
            int x,
            int y,
            int comp,
            byte* data)
        {
            return WriteJpgCore(s, x, y, comp, data, jpgQuality);
        }
    }
}
