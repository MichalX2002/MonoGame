using System;
using System.IO;
using System.Runtime.InteropServices;
using StbSharp;

namespace MonoGame.Imaging
{
    public static unsafe class ImageReader
    {
        public static void* Read(
            bool bit16, Stream stream, out int x, out int y, out int comp, int req_comp)
        {
            var callbacks = new StbImage.stbi_io_callbacks();
            callbacks.read = ReadCallback;
            callbacks.skip = SkipCallback;

            int xx, yy, ccomp;
            void* result;
            if (bit16)
            {
                result = StbImage.stbi_load_16_from_callbacks(
                    callbacks, stream, &xx, &yy, &ccomp, req_comp);
            }
            else
            {
                result = StbImage.stbi_load_from_callbacks(
                    callbacks, stream, &xx, &yy, &ccomp, req_comp);
            }

            x = xx;
            y = yy;
            comp = ccomp;

            if (result == null)
                throw new InvalidOperationException(StbImage.LastError);
            return result;
        }

        public static void* Decode(
            bool bit16, ReadOnlySpan<byte> data, out int x, out int y, out int comp, int req_comp)
        {
            int xx, yy, ccomp;
            void* result;
            fixed (byte* b = &MemoryMarshal.GetReference(data))
            {
                if (bit16)
                {
                    result = StbImage.stbi_load_16_from_memory(
                        b, data.Length, &xx, &yy, &ccomp, req_comp);
                }
                else
                {
                    result = StbImage.stbi_load_from_memory(
                        b, data.Length, &xx, &yy, &ccomp, req_comp);
                }
            }

            x = xx;
            y = yy;
            comp = ccomp;

            if (result == null)
                throw new InvalidOperationException(StbImage.LastError);
            return result;
        }

        private static unsafe int SkipCallback(Stream stream, int n)
        {
            stream.Seek(n, SeekOrigin.Current);
            return n;
        }

        static byte[] buffer = new byte[1024 * 16];

        private static unsafe int ReadCallback(Stream stream, byte* data, int size)
        {
            if (data == null || size <= 0)
                return 0;

            int left = size;
            int read;
            while (left > 0 && (read = stream.Read(buffer, 0, Math.Min(buffer.Length, left))) > 0)
            {
                int totalRead = size - left;
                for (int i = 0; i < read; i++)
                    data[i + totalRead] = buffer[i];

                //_infoBuffer?.Write(buffer, 0, read);
                left -= read;
            }

            // "size - left" gives us how much we've already read
            return size - left;
        }
    }
}
