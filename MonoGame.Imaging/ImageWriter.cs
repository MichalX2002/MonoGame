using System;
using System.IO;

namespace MonoGame.Imaging
{
    public class ImageWriter : IDisposable
    {
        private Stream _stream;
        private int _lastSize;

        [ThreadStatic]
        private static byte[] _buffer;
        
        public bool Disposed { get; private set; }
        public bool LeaveOpen { get; protected set; }

        public ImageWriter(Stream stream, bool leaveOpen)
        {
            _stream = stream;
            LeaveOpen = leaveOpen;
        }

        private unsafe int WriteCallback(void* context, void* data, int size)
        {
            if (data == null || size <= 0)
            {
                _lastSize = 0;
                return 0;
            }

            if(_buffer == null)
                _buffer = new byte[1024 * 64];

            using (var input = new UnmanagedMemoryStream((byte*)data, size))
            {
                int read;
                while ((read = input.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    _stream.Write(_buffer, 0, read);
                }
            }

            _lastSize = size;
            return size;
        }

        public int Write(IntPtr bytes, int width, int height, int sourceChannels, Format format)
        {
            unsafe
            {
                return Write((byte*)bytes, width, height, sourceChannels, format);
            }
        }

        public unsafe int Write(byte* bytes, int width, int height, int sourceChannels, Format format)
        {
            switch (format)
            {
                case Format.Bmp:
                    Imaging.CallbackWriteBmp(WriteCallback, null, width, height, sourceChannels, bytes);
                    break;

                case Format.Tga:
                    Imaging.CallbackWriteTga(WriteCallback, null, width, height, sourceChannels, bytes);
                    break;

                case Format.Jpg:
                    Imaging.CallbackWriteJpg(WriteCallback, null, width, height, sourceChannels, bytes, 90);
                    break;

                case Format.Png:
                    Imaging.CallbackWritePng(WriteCallback, null, width, height, sourceChannels, bytes, width * sourceChannels);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(format), $"Invalid format ({format}).");
            }

            return _lastSize;
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
        
        ~ImageWriter()
        {
            Dispose(false);
        }

        public enum Format
        {
            Bmp,
            Tga,
            Png,
            Jpg,
        }
    }
}
