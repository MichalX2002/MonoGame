using System;
using System.IO;

namespace MonoGame.Imaging
{
    internal class ImageWriter : IDisposable
    {
        private MemoryManager _manager;
        private Stream _stream;
        private int _lastSize;

        [ThreadStatic]
        private static byte[] _buffer;
        
        public bool Disposed { get; private set; }
        public bool LeaveOpen { get; protected set; }

        public ImageWriter(Stream stream, bool leaveOpen)
        {
            _manager = new MemoryManager(true);
            _stream = stream;
            LeaveOpen = leaveOpen;
        }

        private unsafe int WriteCallback(Stream stream, byte* data, int size)
        {
            if (data == null || size <= 0)
            {
                _lastSize = 0;
                return 0;
            }

            if(_buffer == null)
                _buffer = new byte[1024 * 32];

            using (var input = new UnmanagedMemoryStream(data, size))
            {
                int read;
                while ((read = input.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    stream.Write(_buffer, 0, read);
                }
            }

            _lastSize = size;
            return size;
        }

        public int Write(IntPtr bytes, int width, int height, int sourceChannels, ImageSaveFormat format)
        {
            unsafe
            {
                return Write((byte*)bytes, width, height, sourceChannels, format);
            }
        }

        public unsafe int Write(byte* bytes, int width, int height, int sourceChannels, ImageSaveFormat format)
        {
            switch (format)
            {
                case ImageSaveFormat.Bmp:
                    Imaging.CallbackWriteBmp(WriteCallback, _manager, _stream, width, height, sourceChannels, bytes);
                    break;

                case ImageSaveFormat.Tga:
                    Imaging.CallbackWriteTga(WriteCallback, _manager, _stream, width, height, sourceChannels, bytes);
                    break;

                case ImageSaveFormat.Jpg:
                    Imaging.CallbackWriteJpg(WriteCallback, _manager, _stream, width, height, sourceChannels, bytes, 90);
                    break;

                case ImageSaveFormat.Png:
                    Imaging.CallbackWritePng(WriteCallback, _manager, _stream, width, height, sourceChannels, bytes, width * sourceChannels);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(format), $"Invalid Format: ({format}).");
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

                _manager.Dispose();
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
    }
}
