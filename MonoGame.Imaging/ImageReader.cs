using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    internal unsafe class ImageReader : IDisposable
    {
        private MemoryManager _manager;
        private Stream _stream;
        private ReadCallbacks _callbacks;

        [ThreadStatic]
        private static byte[] _buffer;

        public bool Disposed { get; private set; }
        public bool LeaveOpen { get; protected set; }

        public ImageReader(MemoryManager manager, Stream stream, bool leaveOpen)
        {
            _manager = manager;
            _stream = stream;
            LeaveOpen = leaveOpen;

            _callbacks = new ReadCallbacks(ReadCallback, EoFCallback);
        }

        private int SkipCallback(void* user, int i)
        {
            return (int)_stream.Seek(i, SeekOrigin.Current);
        }

        private int EoFCallback(void* user)
        {
            return _stream.CanRead ? 1 : 0;
        }

        private int ReadCallback(void* user, byte* data, int size)
        {
            if(_buffer == null)
                _buffer = new byte[1024 * 32];

            int total = 0;
            using (var stream = new UnmanagedMemoryStream((byte*)data, 0, size, FileAccess.Write))
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
            out int channels, ImagePixelFormat desiredChannels)
        {
            int dc = (int)desiredChannels;

            int xx, yy, cc;
            ErrorContext errorCtx = new ErrorContext();
            var result = Imaging.Load8FromCallbacks(
                _manager, errorCtx, _callbacks, null, &xx, &yy, &cc, dc);

            width = xx;
            height = yy;
            channels = cc;

            if (result == null)
                throw new InvalidDataException(errorCtx.ToString());

            // Convert to array
            int c = (dc >= 1 && dc <= 4) ? dc : channels;
            int size = width * height * c;

            output = Marshal.AllocHGlobal(size);
            Buffer.MemoryCopy(result, (byte*)output, size, size);
            _manager.Free(result);

            return size;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    if (LeaveOpen == false)
                        _stream.Dispose();
                }

                _buffer = null;
                _stream = null;

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageReader()
        {
            Dispose(false);
        }
    }
}
