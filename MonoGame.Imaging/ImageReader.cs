using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    internal unsafe class ImageReader
    {
        private Stream _stream;

        [ThreadStatic]
        private static byte[] _buffer;

        private Imaging.IOCallbacks _callbacks;

        public ImageReader(Stream stream)
        {
            _stream = stream;
            _callbacks = new Imaging.IOCallbacks(ReadCallback, SkipCallback, EoFCallback);
        }

        private int SkipCallback(void* user, int i)
        {
            return (int)_stream.Seek(i, SeekOrigin.Current);
        }

        private int EoFCallback(void* user)
        {
            return _stream.CanRead ? 1 : 0;
        }

        private int ReadCallback(void* user, sbyte* data, int size)
        {
            if(_buffer == null)
                _buffer = new byte[1024 * 32];

            int total = 0;
            using (var stream = new UnmanagedMemoryStream((byte*)data, size))
            {
                int leftToRead = size;
                int read = 0;
                while (leftToRead > 0 && (read = _stream.Read(_buffer, 0, leftToRead)) > 0)
                {
                    stream.Write(_buffer, 0, read);
                    leftToRead -= read;
                    total += read;
                }
            }

            return total;
        }

        public int Read(out IntPtr output, out int width, out int height, out int channels)
        {
            return Read(out output, out width, out height, out channels, 0);
        }

        public int Read(out IntPtr output, out int width, out int height,
            out int channels, int desiredChannels)
        {
            int xx, yy, cc;
            var result = Imaging.Load8FromCallbacks(
                _callbacks, null, &xx, &yy, &cc, desiredChannels);

            width = xx;
            height = yy;
            channels = cc;

            if (result == null)
                throw new InvalidOperationException(Imaging.LastError);

            // Convert to array
            int c = desiredChannels != 0 ? desiredChannels : channels;
            int size = width * height * c;

            output = Marshal.AllocHGlobal(size);
            Buffer.MemoryCopy(result, (byte*)output, size, size);
            Operations.Free(result);

            return size;
        }
    }
}
