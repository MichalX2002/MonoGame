
using System.IO;

namespace MonoGame.Imaging
{
    unsafe partial class Imaging
    {
        public struct WriteContext
        {
            public WriteCallback Write;
            public MemoryManager Manager;
            public Stream Stream;

            public WriteContext(WriteCallback callback, MemoryManager manager, Stream stream)
            {
                Write = callback;
                Manager = manager;
                Stream = stream;
            }
        }

        public static int stbi_write_tga_with_rle = 1;

        public delegate int WriteCallback(Stream stream, byte* data, int size);

        public static WriteContext GetWriteContext(WriteCallback c, MemoryManager manager, Stream stream)
        {
            return new WriteContext(c, manager, stream);
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
                        s.Write(s.Stream, &x, 1);
                        break;
                    }
                    case '2':
                    {
                        var x = v[vindex++];
                        var b = stackalloc byte[2];
                        b[0] = (byte) (x & 0xff);
                        b[1] = (byte) ((x >> 8) & 0xff);
                        s.Write(s.Stream, b, 2);
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
                        s.Write(s.Stream, b, 4);
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

        public static int CallbackWriteBmp(WriteCallback func, MemoryManager manager,
            Stream stream,
            int x,
            int y,
            int comp,
            void* data
            )
        {
            WriteContext s = GetWriteContext(func, manager, stream);
            return WriteBmpCore(s, x, y, comp, data);
        }

        public static int CallbackWriteTga(WriteCallback func, MemoryManager manager,
            Stream stream,
            int x,
            int y,
            int comp,
            void* data
            )
        {
            WriteContext s = GetWriteContext(func, manager, stream);
            return WriteTgaCore(s, x, y, comp, data);
        }

        public static int CallbackWritePng(WriteCallback func, MemoryManager manager,
            Stream stream,
            int x,
            int y,
            int comp,
            void* data,
            int stride_bytes
            )
        {
            int len;
            var png = MemoryWritePng(manager, (byte*) (data), stride_bytes, x, y, comp, &len);
            if (png == null)
                return 0;
            func(stream, png, len);
            manager.Free(png);
            return 1;
        }

        public static int CallbackWriteJpg(WriteCallback func, MemoryManager manager,
            Stream stream,
            int x,
            int y,
            int comp,
            void* data,
            int quality
            )
        {
            WriteContext s = GetWriteContext(func, manager, stream);
            return WriteJpgCore(s, x, y, comp, data, quality);
        }
    }
}
