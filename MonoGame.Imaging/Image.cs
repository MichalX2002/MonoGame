using System;
using System.IO;

namespace MonoGame.Imaging
{
    public sealed class Image : IDisposable
    {
        private readonly object _mutex;

        private Stream _stream;
        private MemoryManager _manager;
        private readonly bool _leaveStreamOpen;
        private readonly bool _leaveManagerOpen;

        private IntPtr _pointer;
        private ImageInfo _cachedInfo;

        private byte[] _buffer;
        private ReadCallbacks _callbacks;
        private ReadContext _context;

        public bool Disposed { get; private set; }
        public IntPtr Pointer => GetDataPointer();
        public ImageInfo Info => GetImageInfo();

        public ImagingError LastError { get; private set; }
        public bool LastGetInfoFailed { get; private set; }
        public bool LastGetContextFailed { get; private set; }
        public bool LastGetPointerFailed { get; private set; }
        
        public Image(
            Stream stream, bool leaveStreamOpen,
            MemoryManager manager, bool leaveManagerOpen)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveStreamOpen = leaveStreamOpen;

            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _leaveManagerOpen = leaveManagerOpen;

            _mutex = new object();

            unsafe
            {
                _callbacks = new ReadCallbacks(ReadCallback, SkipCallback, EoFCallback);
            }
        }

        public Image(Stream stream, bool leaveOpen) :
            this(stream, leaveOpen, new MemoryManager(true), false)
        {
        }

        private unsafe int SkipCallback(void* user, int i)
        {
            return (int)_stream.Seek(i, SeekOrigin.Current);
        }

        private unsafe int EoFCallback(void* user)
        {
            return _stream.CanRead ? 1 : 0;
        }

        private unsafe int ReadCallback(void* user, sbyte* data, int size)
        {
            if (_buffer == null)
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

        private unsafe ReadContext GetReadContext()
        {
            if (_context == null)
            {
                if (LastGetContextFailed == true)
                    return null;

                _context = Imaging.GetReadContext(_manager, _callbacks, null);
            }

            if (_context == null)
            {
                LastGetContextFailed = true;
                LastError = ImagingError.GetLatestError();
            }

            return _context;
        }

        private ImageInfo GetImageInfo()
        {
            if (_cachedInfo == null && LastGetInfoFailed == false)
            {
                unsafe
                {
                    var rc = GetReadContext();
                    _cachedInfo = Imaging.GetImageInfo(rc);

                    if (_cachedInfo.IsValid())
                    {
                        LastGetInfoFailed = true;
                        LastError = ImagingError.GetLatestError();
                    }
                }
            }

            return _cachedInfo;
        }

        public IntPtr GetDataPointer()
        {
            if (_pointer == null)
            {
                if (LastGetPointerFailed == true)
                    return IntPtr.Zero;

                unsafe
                {
                    int width;
                    int height;
                    int comp;

                    var rc = GetReadContext();
                    var data = Imaging.LoadAndPostprocess8(rc, &width, &height, &comp, 4);

                    var info = GetImageInfo();
                    int srcComp = (int)info.PixelFormat;
                    if (info.Width == width && info.Height == height && srcComp == comp)
                    {
                        _pointer = (IntPtr)data;
                    }
                }
            }

            if (_pointer == null)
            {
                LastGetPointerFailed = true;
                LastError = ImagingError.GetLatestError();
            }

            return _pointer;
        }

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                unsafe
                {
                    if (_pointer != null)
                    {
                        _manager.Free((void*)_pointer);
                        _pointer = IntPtr.Zero;
                    }
                }

                if (_leaveManagerOpen == false)
                    _manager.Dispose();

                if (_leaveStreamOpen == false)
                    _stream.Dispose();

                _manager = null;
                _stream = null;
                _buffer = null;

                Disposed = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Image()
        {
            Dispose(false);
        }
    }
}
