using System;
using System.IO;

namespace MonoGame.Imaging
{
    public class ImageWriter : IDisposable
    {
        private Stream _stream;
        private byte[] _buffer;
        private int _lastSize;
        
        public bool Disposed { get; private set; }
        public bool LeaveOpen { get; protected set; }

        public ImageWriter(Stream stream, bool leaveOpen)
        {
            _stream = stream;
            LeaveOpen = leaveOpen;

            _buffer = new byte[1024 * 64];
        }

        private unsafe int WriteCallback(void* context, void* data, int size)
        {
            _lastSize = size;

            if (data == null || size <= 0)
                return 0;

            using (var input = new UnmanagedMemoryStream((byte*)data, size))
            {
                int read;
                while ((read = input.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    _stream.Write(_buffer, 0, read);
                }
            }

            return size;
        }

        public unsafe int Write(byte* bytes, int width, int height, int channels, Format format)
        {
            switch (format)
            {
                case Format.Bmp:
                    Imaging.CallbackWriteBmp(WriteCallback, null, width, height, channels, bytes);
                    break;

                case Format.Tga:
                    Imaging.CallbackWriteTga(WriteCallback, null, width, height, channels, bytes);
                    break;

                case Format.Jpg:
                    Imaging.CallbackWriteJpg(WriteCallback, null, width, height, channels, bytes, 90);
                    break;

                case Format.Png:
                    Imaging.CallbackWritePng(WriteCallback, null, width, height, channels, bytes, width * channels);
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
